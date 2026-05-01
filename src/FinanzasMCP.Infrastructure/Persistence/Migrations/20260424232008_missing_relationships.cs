using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class missing_relationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_transaction_tags_TagId",
                table: "transaction_tags",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_crypto_lots_crypto_accounts_AccountId",
                table: "crypto_lots",
                column: "AccountId",
                principalTable: "crypto_accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_debt_payments_debts_DebtId",
                table: "debt_payments",
                column: "DebtId",
                principalTable: "debts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_goal_contributions_purchase_goals_PurchaseGoalId",
                table: "purchase_goal_contributions",
                column: "PurchaseGoalId",
                principalTable: "purchase_goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_saving_goal_contributions_saving_goals_GoalId",
                table: "saving_goal_contributions",
                column: "GoalId",
                principalTable: "saving_goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_tags_tags_TagId",
                table: "transaction_tags",
                column: "TagId",
                principalTable: "tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_crypto_lots_crypto_accounts_AccountId",
                table: "crypto_lots");

            migrationBuilder.DropForeignKey(
                name: "FK_debt_payments_debts_DebtId",
                table: "debt_payments");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_goal_contributions_purchase_goals_PurchaseGoalId",
                table: "purchase_goal_contributions");

            migrationBuilder.DropForeignKey(
                name: "FK_saving_goal_contributions_saving_goals_GoalId",
                table: "saving_goal_contributions");

            migrationBuilder.DropForeignKey(
                name: "FK_transaction_tags_tags_TagId",
                table: "transaction_tags");

            migrationBuilder.DropIndex(
                name: "IX_transaction_tags_TagId",
                table: "transaction_tags");
        }
    }
}
