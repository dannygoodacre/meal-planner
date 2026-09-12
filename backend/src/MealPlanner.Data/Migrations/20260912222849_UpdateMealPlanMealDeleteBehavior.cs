using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealPlanner.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMealPlanMealDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealPlanMeals_MealPlans_MealPlanId",
                table: "MealPlanMeals");

            migrationBuilder.AddForeignKey(
                name: "FK_MealPlanMeals_MealPlans_MealPlanId",
                table: "MealPlanMeals",
                column: "MealPlanId",
                principalTable: "MealPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealPlanMeals_MealPlans_MealPlanId",
                table: "MealPlanMeals");

            migrationBuilder.AddForeignKey(
                name: "FK_MealPlanMeals_MealPlans_MealPlanId",
                table: "MealPlanMeals",
                column: "MealPlanId",
                principalTable: "MealPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
