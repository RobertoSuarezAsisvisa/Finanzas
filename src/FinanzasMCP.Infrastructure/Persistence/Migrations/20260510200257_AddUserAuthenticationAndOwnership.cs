using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAuthenticationAndOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_context_Key",
                table: "user_context");

            migrationBuilder.DropIndex(
                name: "IX_tags_Name",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "IX_categories_Name_ParentId",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_budgets_Name_PeriodType_ValidityType_PeriodStart",
                table: "budgets");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "user_context",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "transaction_tags",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "tags",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "saving_goals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "saving_goal_contributions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "recurring_rules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "purchase_goals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "purchase_goal_contributions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "debts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "debt_payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "debt_installments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "crypto_lots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "crypto_accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "categories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "budgets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "accounting_periods",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_external_logins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProviderUserId = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_external_logins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_external_logins_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_context_UserId",
                table: "user_context",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_context_UserId_Key",
                table: "user_context",
                columns: new[] { "UserId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_UserId",
                table: "transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_tags_UserId",
                table: "transaction_tags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tags_UserId",
                table: "tags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tags_UserId_Name",
                table: "tags",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_saving_goals_UserId",
                table: "saving_goals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_saving_goal_contributions_UserId",
                table: "saving_goal_contributions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_recurring_rules_UserId",
                table: "recurring_rules",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_goals_UserId",
                table: "purchase_goals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_goal_contributions_UserId",
                table: "purchase_goal_contributions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_debts_UserId",
                table: "debts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_debt_payments_UserId",
                table: "debt_payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_debt_installments_UserId",
                table: "debt_installments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_crypto_lots_UserId",
                table: "crypto_lots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_crypto_accounts_UserId",
                table: "crypto_accounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_categories_UserId",
                table: "categories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_categories_UserId_Name_ParentId",
                table: "categories",
                columns: new[] { "UserId", "Name", "ParentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_budgets_UserId",
                table: "budgets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_budgets_UserId_Name_PeriodType_ValidityType_PeriodStart",
                table: "budgets",
                columns: new[] { "UserId", "Name", "PeriodType", "ValidityType", "PeriodStart" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_accounts_UserId",
                table: "accounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_accounting_periods_UserId",
                table: "accounting_periods",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_external_logins_Provider_ProviderUserId",
                table: "user_external_logins",
                columns: new[] { "Provider", "ProviderUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_external_logins_UserId",
                table: "user_external_logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_external_logins");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropIndex(
                name: "IX_user_context_UserId",
                table: "user_context");

            migrationBuilder.DropIndex(
                name: "IX_user_context_UserId_Key",
                table: "user_context");

            migrationBuilder.DropIndex(
                name: "IX_transactions_UserId",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transaction_tags_UserId",
                table: "transaction_tags");

            migrationBuilder.DropIndex(
                name: "IX_tags_UserId",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "IX_tags_UserId_Name",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "IX_saving_goals_UserId",
                table: "saving_goals");

            migrationBuilder.DropIndex(
                name: "IX_saving_goal_contributions_UserId",
                table: "saving_goal_contributions");

            migrationBuilder.DropIndex(
                name: "IX_recurring_rules_UserId",
                table: "recurring_rules");

            migrationBuilder.DropIndex(
                name: "IX_purchase_goals_UserId",
                table: "purchase_goals");

            migrationBuilder.DropIndex(
                name: "IX_purchase_goal_contributions_UserId",
                table: "purchase_goal_contributions");

            migrationBuilder.DropIndex(
                name: "IX_debts_UserId",
                table: "debts");

            migrationBuilder.DropIndex(
                name: "IX_debt_payments_UserId",
                table: "debt_payments");

            migrationBuilder.DropIndex(
                name: "IX_debt_installments_UserId",
                table: "debt_installments");

            migrationBuilder.DropIndex(
                name: "IX_crypto_lots_UserId",
                table: "crypto_lots");

            migrationBuilder.DropIndex(
                name: "IX_crypto_accounts_UserId",
                table: "crypto_accounts");

            migrationBuilder.DropIndex(
                name: "IX_categories_UserId",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_UserId_Name_ParentId",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_budgets_UserId",
                table: "budgets");

            migrationBuilder.DropIndex(
                name: "IX_budgets_UserId_Name_PeriodType_ValidityType_PeriodStart",
                table: "budgets");

            migrationBuilder.DropIndex(
                name: "IX_accounts_UserId",
                table: "accounts");

            migrationBuilder.DropIndex(
                name: "IX_accounting_periods_UserId",
                table: "accounting_periods");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "user_context");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "transaction_tags");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "saving_goals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "saving_goal_contributions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "recurring_rules");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "purchase_goals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "purchase_goal_contributions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "debts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "debt_payments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "debt_installments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "crypto_lots");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "crypto_accounts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "budgets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "accounting_periods");

            migrationBuilder.CreateIndex(
                name: "IX_user_context_Key",
                table: "user_context",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tags_Name",
                table: "tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_Name_ParentId",
                table: "categories",
                columns: new[] { "Name", "ParentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_budgets_Name_PeriodType_ValidityType_PeriodStart",
                table: "budgets",
                columns: new[] { "Name", "PeriodType", "ValidityType", "PeriodStart" },
                unique: true);
        }
    }
}
