using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_budget_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_budgets_CategoryId_PeriodType_ValidityType_PeriodStart",
                table: "budgets");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "budgets",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_budgets_CategoryId",
                table: "budgets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_budgets_Name_CategoryId_PeriodType_ValidityType_PeriodStart",
                table: "budgets",
                columns: new[] { "Name", "CategoryId", "PeriodType", "ValidityType", "PeriodStart" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_budgets_CategoryId",
                table: "budgets");

            migrationBuilder.DropIndex(
                name: "IX_budgets_Name_CategoryId_PeriodType_ValidityType_PeriodStart",
                table: "budgets");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "budgets");

            migrationBuilder.CreateIndex(
                name: "IX_budgets_CategoryId_PeriodType_ValidityType_PeriodStart",
                table: "budgets",
                columns: new[] { "CategoryId", "PeriodType", "ValidityType", "PeriodStart" },
                unique: true);
        }
    }
}
