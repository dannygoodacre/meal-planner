namespace MealPlanner.Application.Entities;

public abstract class PublicEntity
{
    public int Id { get; init; }

    public required Guid PublicId { get; init; }

    public required string Name { get; set; }

    public required string NormalizedName { get; set; }
}
