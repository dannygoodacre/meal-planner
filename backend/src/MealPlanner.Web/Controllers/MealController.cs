using DannyGoodacre.Primitives;
using MealPlanner.Application.Commands;
using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;
using MealPlanner.Web.Models;
using Microsoft.AspNetCore.Mvc;
using IResult = DannyGoodacre.Primitives.IResult;

namespace MealPlanner.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MealController(IAddMeal addMeal,
                                   IGetMeal getMeal,
                                   IUpdateMeal updateMeal,
                                   IDeleteMeal deleteMeal,
                                   ISearchMealByName searchMealByName) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddMealAsync([FromBody] AddMealRequest request, CancellationToken cancellationToken)
    {
        IResult<Guid> result = await addMeal.ExecuteAsync(request.ToCommand(), cancellationToken);

        return result is Success<Guid>(var value)
            ? CreatedAtAction(nameof(GetAsync), new { id = value }, value)
            : result.ToHttpResponse();
    }

    [HttpGet("{id:guid}")]
    [ActionName(nameof(GetAsync))]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        IResult<MealResponse> result = await getMeal.ExecuteAsync(id, cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateMealRequest request, CancellationToken cancellationToken)
    {
        IResult<Guid> result = await updateMeal.ExecuteAsync(request.ToCommand(id), cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        IResult result = await deleteMeal.ExecuteAsync(id, cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAsync([FromQuery(Name = "q")] string searchTerm, CancellationToken cancellationToken)
    {
        IResult<SearchResponse<MealResponse>> result = await searchMealByName.ExecuteAsync(searchTerm, cancellationToken);

        return result.ToHttpResponse();
    }
}
