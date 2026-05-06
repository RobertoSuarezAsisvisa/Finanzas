using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BudgetId",
                table: "transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_BudgetId",
                table: "transactions",
                column: "BudgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_budgets_BudgetId",
                table: "transactions",
                column: "BudgetId",
                principalTable: "budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_budgets_BudgetId",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_BudgetId",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "transactions");
        }
    }
}
