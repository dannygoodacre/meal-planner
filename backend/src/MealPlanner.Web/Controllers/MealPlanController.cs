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
[Route("api/meal-plans")]
public sealed class MealPlanController(ICommandHandler<AddMealPlanCommand, Guid> addMealPlan,
                                      IQueryHandler<GetMealPlanQuery, MealPlanResponse> getMealPlan,
                                      ICommandHandler<UpdateMealPlanCommand> updateMealPlan,
                                      ICommandHandler<DeleteMealPlanCommand> deleteMealPlan,
                                      IQueryHandler<SearchMealPlanQuery, SearchResponse<MealPlanResponse>> searchMealPlan) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddMealPlanAsync([FromBody] AddMealPlanRequest request, CancellationToken cancellationToken)
    {
        IResult<Guid> result = await addMealPlan.HandleAsync(request.ToCommand(), cancellationToken);

        return result is Success<Guid>(var value)
            ? CreatedAtAction(nameof(GetAsync), new { id = value }, value)
            : result.ToHttpResponse();
    }

    [HttpGet("{id:guid}")]
    [ActionName(nameof(GetAsync))]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        IResult<MealPlanResponse> result = await getMealPlan.HandleAsync(new GetMealPlanQuery(id), cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateMealPlanRequest request, CancellationToken cancellationToken)
    {
        IResult result = await updateMealPlan.HandleAsync(request.ToCommand(id), cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        IResult result = await deleteMealPlan.HandleAsync(new DeleteMealPlanCommand(id), cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery(Name = "q")] string searchTerm, CancellationToken cancellationToken)
    {
        IResult<SearchResponse<MealPlanResponse>> result = await searchMealPlan.HandleAsync(new SearchMealPlanQuery(searchTerm), cancellationToken);

        return result.ToHttpResponse();
    }
}
