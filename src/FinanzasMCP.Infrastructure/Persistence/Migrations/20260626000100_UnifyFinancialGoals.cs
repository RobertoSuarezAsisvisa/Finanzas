using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(FinanzasMCPDbContext))]
    [Migration("20260626000100_UnifyFinancialGoals")]
    public partial class UnifyFinancialGoals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "financial_goals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TargetAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_financial_goals_accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "financial_goal_contributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GoalId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ContributionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_goal_contributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_financial_goal_contributions_financial_goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "financial_goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_financial_goal_contributions_transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.Sql("""
                INSERT INTO financial_goals (
                    "Id", "Name", "Description", "TargetAmount", "CurrentAmount", "AccountId", "TargetDate",
                    "Status", "Type", "Priority", "Url", "CompletedAt", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId")
                SELECT
                    "Id", "Name", NULL, "GoalAmount", "CurrentAmount", "AccountId", "GoalDate",
                    CASE "Status"
                        WHEN 'Completed' THEN 'Completed'
                        WHEN 'Cancelled' THEN 'Cancelled'
                        ELSE 'InProgress'
                    END,
                    'Saving', 1, NULL, NULL, "CreatedAt", "UpdatedAt", "DeletedAt", "UserId"
                FROM saving_goals;

                INSERT INTO financial_goals (
                    "Id", "Name", "Description", "TargetAmount", "CurrentAmount", "AccountId", "TargetDate",
                    "Status", "Type", "Priority", "Url", "CompletedAt", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId")
                SELECT
                    "Id", "Name", "Description", "GoalPrice", "SavedAmount", "AccountId", "TargetDate",
                    CASE "Status"
                        WHEN 'Ready' THEN 'Ready'
                        WHEN 'Purchased' THEN 'Completed'
                        WHEN 'Cancelled' THEN 'Cancelled'
                        ELSE 'InProgress'
                    END,
                    'Purchase', "Priority", "Url", "PurchasedAt", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId"
                FROM purchase_goals;

                INSERT INTO financial_goal_contributions (
                    "Id", "GoalId", "TransactionId", "Amount", "ContributionDate", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId")
                SELECT "Id", "GoalId", "TransactionId", "Amount", "ContributionDate", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId"
                FROM saving_goal_contributions;

                INSERT INTO financial_goal_contributions (
                    "Id", "GoalId", "TransactionId", "Amount", "ContributionDate", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId")
                SELECT "Id", "PurchaseGoalId", "TransactionId", "Amount", "ContributionDate", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId"
                FROM purchase_goal_contributions;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_financial_goal_contributions_GoalId",
                table: "financial_goal_contributions",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_financial_goal_contributions_TransactionId",
                table: "financial_goal_contributions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_financial_goal_contributions_UserId",
                table: "financial_goal_contributions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_financial_goals_AccountId",
                table: "financial_goals",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_financial_goals_Status",
                table: "financial_goals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_financial_goals_Type_Status_Priority",
                table: "financial_goals",
                columns: new[] { "Type", "Status", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_financial_goals_UserId",
                table: "financial_goals",
                column: "UserId");

            migrationBuilder.DropTable(name: "purchase_goal_contributions");
            migrationBuilder.DropTable(name: "saving_goal_contributions");
            migrationBuilder.DropTable(name: "purchase_goals");
            migrationBuilder.DropTable(name: "saving_goals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "purchase_goals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    GoalPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SavedAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PurchasedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_goals", x => x.Id);
                    table.ForeignKey("FK_purchase_goals_accounts_AccountId", x => x.AccountId, "accounts", "Id");
                });

            migrationBuilder.CreateTable(
                name: "saving_goals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GoalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    GoalDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saving_goals", x => x.Id);
                    table.ForeignKey("FK_saving_goals_accounts_AccountId", x => x.AccountId, "accounts", "Id");
                });

            migrationBuilder.CreateTable(
                name: "purchase_goal_contributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseGoalId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ContributionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_goal_contributions", x => x.Id);
                    table.ForeignKey("FK_purchase_goal_contributions_purchase_goals_PurchaseGoalId", x => x.PurchaseGoalId, "purchase_goals", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "saving_goal_contributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GoalId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ContributionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saving_goal_contributions", x => x.Id);
                    table.ForeignKey("FK_saving_goal_contributions_saving_goals_GoalId", x => x.GoalId, "saving_goals", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql("""
                INSERT INTO saving_goals ("Id", "Name", "GoalAmount", "CurrentAmount", "AccountId", "GoalDate", "Status", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId")
                SELECT "Id", "Name", "TargetAmount", "CurrentAmount", "AccountId", "TargetDate",
                    CASE "Status" WHEN 'Completed' THEN 'Completed' WHEN 'Cancelled' THEN 'Cancelled' ELSE 'InProgress' END,
                    "CreatedAt", "UpdatedAt", "DeletedAt", "UserId"
                FROM financial_goals
                WHERE "Type" = 'Saving';

                INSERT INTO purchase_goals ("Id", "Name", "Description", "GoalPrice", "SavedAmount", "Priority", "Url", "AccountId", "TargetDate", "Status", "PurchasedAt", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId")
                SELECT "Id", "Name", "Description", "TargetAmount", "CurrentAmount", "Priority", "Url", "AccountId", "TargetDate",
                    CASE "Status" WHEN 'Ready' THEN 'Ready' WHEN 'Completed' THEN 'Purchased' WHEN 'Cancelled' THEN 'Cancelled' ELSE 'Saving' END,
                    "CompletedAt", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId"
                FROM financial_goals
                WHERE "Type" = 'Purchase';

                INSERT INTO saving_goal_contributions ("Id", "GoalId", "TransactionId", "Amount", "ContributionDate", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId")
                SELECT c."Id", c."GoalId", c."TransactionId", c."Amount", c."ContributionDate", c."CreatedAt", c."UpdatedAt", c."DeletedAt", c."UserId"
                FROM financial_goal_contributions c
                INNER JOIN financial_goals g ON g."Id" = c."GoalId"
                WHERE g."Type" = 'Saving';

                INSERT INTO purchase_goal_contributions ("Id", "PurchaseGoalId", "TransactionId", "Amount", "ContributionDate", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId")
                SELECT c."Id", c."GoalId", c."TransactionId", c."Amount", c."ContributionDate", c."CreatedAt", c."UpdatedAt", c."DeletedAt", c."UserId"
                FROM financial_goal_contributions c
                INNER JOIN financial_goals g ON g."Id" = c."GoalId"
                WHERE g."Type" = 'Purchase';
                """);

            migrationBuilder.DropTable(name: "financial_goal_contributions");
            migrationBuilder.DropTable(name: "financial_goals");
        }
    }
}
