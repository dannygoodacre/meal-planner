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
[Route("api/meals")]
public sealed class MealController(ICommandHandler<AddMealCommand, Guid> addMeal,
                                  IQueryHandler<GetMealQuery, MealResponse> getMeal,
                                  ICommandHandler<UpdateMealCommand> updateMeal,
                                  ICommandHandler<DeleteMealCommand> deleteMeal,
                                  IQueryHandler<SearchMealQuery, SearchResponse<MealResponse>> searchMeal) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddMealAsync([FromBody] AddMealRequest request, CancellationToken cancellationToken)
    {
        IResult<Guid> result = await addMeal.HandleAsync(request.ToCommand(), cancellationToken);

        return result is Success<Guid>(var value)
            ? CreatedAtAction(nameof(GetAsync), new { id = value }, value)
            : result.ToHttpResponse();
    }

    [HttpGet("{id:guid}")]
    [ActionName(nameof(GetAsync))]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        IResult<MealResponse> result = await getMeal.HandleAsync(new GetMealQuery(id), cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateMealRequest request, CancellationToken cancellationToken)
    {
        IResult result = await updateMeal.HandleAsync(request.ToCommand(id), cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        IResult result = await deleteMeal.HandleAsync(new DeleteMealCommand(id), cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery(Name = "q")] string searchTerm, CancellationToken cancellationToken)
    {
        IResult<SearchResponse<MealResponse>> result = await searchMeal.HandleAsync(new SearchMealQuery(searchTerm), cancellationToken);

        return result.ToHttpResponse();
    }
}
