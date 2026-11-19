using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Commands;
using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;
using MealPlanner.Web.Models;
using Microsoft.AspNetCore.Mvc;
using IResult = DannyGoodacre.Results.IResult;

namespace MealPlanner.Web.Controllers;

[ApiController]
[Route("api/foods")]
public sealed class FoodController(ICommandHandler<AddFoodCommand, Guid> addFood,
                                  IQueryHandler<GetFoodQuery, FoodResponse> getFood,
                                  ICommandHandler<UpdateFoodCommand> updateFood,
                                  ICommandHandler<DeleteFoodCommand> deleteFood,
                                  IQueryHandler<SearchFoodQuery, SearchResponse<FoodResponse>> searchFood) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] AddFoodRequest request, CancellationToken cancellationToken)
    {
        IResult<Guid> result = await addFood.HandleAsync(request.ToCommand(), cancellationToken);

        return result is Success<Guid>(var value)
            ? CreatedAtAction(nameof(GetAsync), new { id = value }, value)
            : result.ToHttpResponse();
    }

    [HttpGet("{id:guid}")]
    [ActionName(nameof(GetAsync))]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        IResult<FoodResponse> result = await getFood.HandleAsync(new GetFoodQuery(id), cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateFoodRequest request, CancellationToken cancellationToken)
    {
        IResult result = await updateFood.HandleAsync(request.ToCommand(id), cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        IResult result = await deleteFood.HandleAsync(new DeleteFoodCommand(id), cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery(Name = "q")] string searchTerm, CancellationToken cancellationToken)
    {
        IResult<SearchResponse<FoodResponse>> result = await searchFood.HandleAsync(new SearchFoodQuery(searchTerm), cancellationToken);

        return result.ToHttpResponse();
    }
}
