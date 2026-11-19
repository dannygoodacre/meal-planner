using DannyGoodacre.Cqrs;
using MealPlanner.Application.Commands;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;
using MealPlanner.Data;
using MealPlanner.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner.IntegrationTests.Pipelines;

public abstract class PipelineTestBase(PostgresFixture postgres) : IntegrationTestBase(postgres)
{
    protected static CancellationToken Ct => TestContext.Current.CancellationToken;

    protected Task<IResult<Guid>> AddAsync<TCommand>(TCommand command) where TCommand : ICommand
        => Host.InScopeAsync(sp => sp.GetRequiredService<ICommandHandler<TCommand, Guid>>().HandleAsync(command, Ct));

    protected Task<IResult> SendAsync<TCommand>(TCommand command) where TCommand : ICommand
        => Host.InScopeAsync(sp => sp.GetRequiredService<ICommandHandler<TCommand>>().HandleAsync(command, Ct));

    protected Task<IResult<TResult>> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult> where TResult : notnull
        => Host.InScopeAsync(sp => sp.GetRequiredService<IQueryHandler<TQuery, TResult>>().HandleAsync(query, Ct));

    /// <summary>
    /// Reads from a new scope, so the assertions see what was committed rather than what a change tracker holds.
    /// </summary>
    protected Task<T> ReadAsync<T>(Func<ApplicationContext, Task<T>> read)
        => Host.InScopeAsync(sp => read(sp.GetRequiredService<ApplicationContext>()));

    protected Task<int> CountFoodsAsync() => ReadAsync(c => c.Foods.CountAsync(Ct));

    protected Task<int> CountMealsAsync() => ReadAsync(c => c.Meals.CountAsync(Ct));

    protected Task<int> CountMealPlansAsync() => ReadAsync(c => c.MealPlans.CountAsync(Ct));

    /// <summary>
    /// The meal's ingredients as (food public ID, quantity).
    /// </summary>
    protected Task<List<(Guid FoodId, int Quantity)>> MealIngredientsAsync(Guid mealId)
        => ReadAsync(async c => (await c.Set<Ingredient>()
                .Where(x => x.Meal.PublicId == mealId)
                .Select(x => new { x.Food.PublicId, x.Quantity })
                .ToListAsync(Ct))
            .Select(x => (x.PublicId, x.Quantity)).ToList());

    /// <summary>
    /// The plan's meals as (meal public ID, name, ingredients as (food public ID, quantity)), ordered by name.
    /// </summary>
    protected Task<List<(Guid Id, string Name, List<(Guid FoodId, int Quantity)> Ingredients)>> PlanMealsAsync(Guid planId)
        => ReadAsync(async c => (await c.Set<MealPlanMeal>()
                .Where(x => x.MealPlan.PublicId == planId)
                .Select(m => new
                {
                    m.PublicId,
                    m.Name,
                    Ingredients = m.Ingredients.Select(i => new { i.Food.PublicId, i.Quantity }).ToList()
                })
                .ToListAsync(Ct))
            .OrderBy(m => m.Name)
            .Select(m => (m.PublicId, m.Name, m.Ingredients.OrderBy(i => i.Quantity).Select(i => (i.PublicId, i.Quantity)).ToList()))
            .ToList());

    protected static AddFoodCommand ValidAddFood(string name = "Banana") => new()
    {
        Name = name,
        Unit = Unit.Grams,
        ReferenceQuantity = 100,
        Calories = 89,
        Protein = 1,
        Carbohydrates = 23,
        Sugar = 12,
        Fat = 0.3m,
        SaturatedFat = 0.1m,
        Fibre = 2.6m,
        Salt = 0,
        Source = "https://example.com/banana"
    };

    protected static UpdateFoodCommand ValidUpdateFood(Guid id, string name = "Banana") => new()
    {
        Id = id,
        Name = name,
        Unit = Unit.Grams,
        ReferenceQuantity = 100,
        Calories = 89,
        Protein = 1,
        Carbohydrates = 23,
        Sugar = 12,
        Fat = 0.3m,
        SaturatedFat = 0.1m,
        Fibre = 2.6m,
        Salt = 0,
        Source = "https://example.com/banana"
    };
}
