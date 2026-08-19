using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MealPlanner.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                name: "Foods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Unit = table.Column<int>(type: "integer", nullable: false),
                    ReferenceQuantity = table.Column<int>(type: "integer", nullable: false),
                    Calories = table.Column<decimal>(type: "numeric", nullable: false),
                    Protein = table.Column<decimal>(type: "numeric", nullable: false),
                    Carbohydrates = table.Column<decimal>(type: "numeric", nullable: false),
                    Sugar = table.Column<decimal>(type: "numeric", nullable: false),
                    Fat = table.Column<decimal>(type: "numeric", nullable: false),
                    SaturatedFat = table.Column<decimal>(type: "numeric", nullable: false),
                    Fibre = table.Column<decimal>(type: "numeric", nullable: false),
                    Salt = table.Column<decimal>(type: "numeric", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealPlanMeals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MealPlanId = table.Column<int>(type: "integer", nullable: false),
                    PublicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlanMeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealPlanMeals_MealPlans_MealPlanId",
                        column: x => x.MealPlanId,
                        principalTable: "MealPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    FoodId = table.Column<int>(type: "integer", nullable: false),
                    MealId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => new { x.MealId, x.FoodId });
                    table.ForeignKey(
                        name: "FK_Ingredients_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ingredients_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealPlanIngredients",
                columns: table => new
                {
                    FoodId = table.Column<int>(type: "integer", nullable: false),
                    MealPlanMealId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlanIngredients", x => new { x.MealPlanMealId, x.FoodId });
                    table.ForeignKey(
                        name: "FK_MealPlanIngredients_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealPlanIngredients_MealPlanMeals_MealPlanMealId",
                        column: x => x.MealPlanMealId,
                        principalTable: "MealPlanMeals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Food_NormalizedName_Trgm",
                table: "Foods",
                column: "NormalizedName")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Food_NormalizedName_Unique",
                table: "Foods",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Foods_PublicId",
                table: "Foods",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_FoodId",
                table: "Ingredients",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanIngredients_FoodId",
                table: "MealPlanIngredients",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanMeal_NormalizedName_Trgm",
                table: "MealPlanMeals",
                column: "NormalizedName")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanMeals_MealPlanId",
                table: "MealPlanMeals",
                column: "MealPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanMeals_PublicId",
                table: "MealPlanMeals",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealPlan_NormalizedName_Trgm",
                table: "MealPlans",
                column: "NormalizedName")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_MealPlan_NormalizedName_Unique",
                table: "MealPlans",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealPlans_PublicId",
                table: "MealPlans",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meal_NormalizedName_Trgm",
                table: "Meals",
                column: "NormalizedName")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Meal_NormalizedName_Unique",
                table: "Meals",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meals_PublicId",
                table: "Meals",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "MealPlanIngredients");

            migrationBuilder.DropTable(
                name: "Meals");

            migrationBuilder.DropTable(
                name: "Foods");

            migrationBuilder.DropTable(
                name: "MealPlanMeals");

            migrationBuilder.DropTable(
                name: "MealPlans");
        }
    }
}
