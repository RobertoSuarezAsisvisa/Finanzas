using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class renaming_date_names : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetDate",
                table: "saving_goals",
                newName: "GoalDate");

            migrationBuilder.RenameColumn(
                name: "TargetAmount",
                table: "saving_goals",
                newName: "GoalAmount");

            migrationBuilder.RenameColumn(
                name: "TargetPrice",
                table: "purchase_goals",
                newName: "GoalPrice");

            migrationBuilder.CreateIndex(
                name: "IX_saving_goal_contributions_TransactionId",
                table: "saving_goal_contributions",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_saving_goal_contributions_transactions_TransactionId",
                table: "saving_goal_contributions",
                column: "TransactionId",
                principalTable: "transactions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_saving_goal_contributions_transactions_TransactionId",
                table: "saving_goal_contributions");

            migrationBuilder.DropIndex(
                name: "IX_saving_goal_contributions_TransactionId",
                table: "saving_goal_contributions");

            migrationBuilder.RenameColumn(
                name: "GoalDate",
                table: "saving_goals",
                newName: "TargetDate");

            migrationBuilder.RenameColumn(
                name: "GoalAmount",
                table: "saving_goals",
                newName: "TargetAmount");

            migrationBuilder.RenameColumn(
                name: "GoalPrice",
                table: "purchase_goals",
                newName: "TargetPrice");
        }
    }
}
