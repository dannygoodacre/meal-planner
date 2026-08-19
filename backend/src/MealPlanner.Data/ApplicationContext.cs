using DannyGoodacre.Cqrs;
using MealPlanner.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MealPlanner.Data;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options), IStateUnit
{
    public DbSet<Food> Foods { get; set; }

    public DbSet<Meal> Meals { get; set; }

    public DbSet<MealPlan> MealPlans { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresExtension("pg_trgm");

        IEnumerable<IMutableEntityType> publicEntityTypes = builder.Model.GetEntityTypes()
            .Where(x => typeof(PublicEntity).IsAssignableFrom(x.ClrType));

        foreach (IMutableEntityType publicEntityType in publicEntityTypes)
        {
            EntityTypeBuilder entity = builder.Entity(publicEntityType.ClrType);

            entity.HasKey(nameof(PublicEntity.Id));

            entity.HasIndex(nameof(PublicEntity.PublicId))
                .IsUnique();

            if (typeof(IUniqueRegistryItem).IsAssignableFrom(publicEntityType.ClrType))
            {
                entity.HasIndex([nameof(PublicEntity.NormalizedName)], $"IX_{publicEntityType.ClrType.Name}_NormalizedName_Unique")
                        .IsUnique();
            }

            entity.HasIndex([nameof(PublicEntity.NormalizedName)], $"IX_{publicEntityType.ClrType.Name}_NormalizedName_Trgm")
                .HasMethod("gin")
                .HasOperators("gin_trgm_ops");
        }

        builder.Entity<Ingredient>(entity =>
        {
            entity.ToTable("Ingredients");

            entity.HasKey(x => new { x.MealId, x.FoodId });

            entity.HasOne(x => x.Meal)
                .WithMany(x => x.Ingredients)
                .HasForeignKey(x => x.MealId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Food)
                .WithMany()
                .HasForeignKey(x => x.FoodId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<MealPlanIngredient>(entity =>
        {
            entity.ToTable("MealPlanIngredients");

            entity.HasKey(x => new { x.MealPlanMealId, x.FoodId });

            entity.HasOne(x => x.MealPlanMeal)
                .WithMany(x => x.Ingredients)
                .HasForeignKey(x => x.MealPlanMealId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Food)
                .WithMany()
                .HasForeignKey(x => x.FoodId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<MealPlanMeal>(entity =>
        {
            entity.ToTable("MealPlanMeals");

            entity.HasMany(x => x.Ingredients)
                .WithOne(x => x.MealPlanMeal)
                .HasForeignKey(x => x.MealPlanMealId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.MealPlan)
                .WithMany(x => x.Meals)
                .HasForeignKey(x => x.MealPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
