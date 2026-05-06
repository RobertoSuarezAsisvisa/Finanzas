using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DecoupleBudgetsFromCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_budgets_categories_CategoryId",
                table: "budgets");

            migrationBuilder.DropIndex(
                name: "IX_budgets_Name_CategoryId_PeriodType_ValidityType_PeriodStart",
                table: "budgets");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "budgets",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_budgets_Name_PeriodType_ValidityType_PeriodStart",
                table: "budgets",
                columns: new[] { "Name", "PeriodType", "ValidityType", "PeriodStart" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_budgets_categories_CategoryId",
                table: "budgets",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_budgets_categories_CategoryId",
                table: "budgets");

            migrationBuilder.DropIndex(
                name: "IX_budgets_Name_PeriodType_ValidityType_PeriodStart",
                table: "budgets");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "budgets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_budgets_Name_CategoryId_PeriodType_ValidityType_PeriodStart",
                table: "budgets",
                columns: new[] { "Name", "CategoryId", "PeriodType", "ValidityType", "PeriodStart" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_budgets_categories_CategoryId",
                table: "budgets",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
