using DannyGoodacre.Primitives;
using MealPlanner.Application.Commands;
using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;
using MealPlanner.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MealPlanController(IAddMealPlan addMealPlan,
                                       IGetMealPlan getMealPlan,
                                       IUpdateMealPlan updateMealPlan,
                                       IDeleteMealPlan deleteMealPlan,
                                       ISearchMealPlanByName searchMealPlanByName) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddMealPlanAsync([FromBody] AddMealPlanRequest request, CancellationToken cancellationToken)
    {
        Result<Guid> result = await addMealPlan.ExecuteAsync(request.ToCommand(), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAsync), new { id = result.Value }, result.Value)
            : result.ToHttpResponse();
    }

    [HttpGet("{id:guid}")]
    [ActionName(nameof(GetAsync))]
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        Result<MealPlanResponse> result = await getMealPlan.ExecuteAsync(id, cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateMealPlanRequest request, CancellationToken cancellationToken)
    {
        Result<Guid> result = await updateMealPlan.ExecuteAsync(request.ToCommand(id), cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        Result result = await deleteMealPlan.ExecuteAsync(id, cancellationToken);

        return result.ToHttpResponse();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAsync([FromQuery(Name = "q")] string searchTerm, CancellationToken cancellationToken)
    {
        Result<SearchResponse<MealPlanResponse>> result = await searchMealPlanByName.ExecuteAsync(searchTerm, cancellationToken);

        return result.ToHttpResponse();
    }
}
