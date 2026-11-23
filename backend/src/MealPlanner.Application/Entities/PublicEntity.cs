namespace MealPlanner.Application.Entities;

public abstract class PublicEntity
{
    private string _name = null!;

    public int Id { get; init; }

    public required Guid PublicId { get; init; }

    public required string Name
    {
        get => _name;
        set
        {
            _name = value;
            NormalizedName = value.ToNormalizedString();
        }
    }

    public string NormalizedName { get; private set; } = string.Empty;
}
