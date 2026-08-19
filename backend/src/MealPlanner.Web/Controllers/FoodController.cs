using DannyGoodacre.Primitives;
using MealPlanner.Application.Commands;
using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;
using MealPlanner.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class FoodController(IAddFood addFood,
                                   IGetFood getFood,
                                   IUpdateFood updateFood,
                                   IDeleteFood deleteFood,
                                   ISearchFoodByName searchFoodByName) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] AddFoodRequest request, CancellationToken cancellationToken)
    {
        Result<Guid> result = await addFood.ExecuteAsync(request.ToCommand(), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAsync), new { id = result.Value }, result.Value)
            : result.ToHttpResponse();
    }

    [HttpGet("{id:guid}")]
    [ActionName(nameof(GetAsync))]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<FoodResponse> result = await getFood.ExecuteAsync(id, cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateFoodRequest request, CancellationToken cancellationToken)
    {
        Result<Guid> result = await updateFood.ExecuteAsync(request.ToCommand(id), cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        Result result = await deleteFood.ExecuteAsync(id, cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAsync([FromQuery(Name = "q")] string searchTerm, CancellationToken cancellationToken)
    {
        Result<SearchResponse<FoodResponse>> result = await searchFoodByName.ExecuteAsync(searchTerm, cancellationToken);

        return result.ToHttpResponse();
    }
}
