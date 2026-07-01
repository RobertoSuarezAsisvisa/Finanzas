using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBudgetCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_budgets_categories_CategoryId",
                table: "budgets");

            migrationBuilder.DropIndex(
                name: "IX_budgets_CategoryId",
                table: "budgets");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "budgets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "budgets",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_budgets_CategoryId",
                table: "budgets",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_budgets_categories_CategoryId",
                table: "budgets",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
