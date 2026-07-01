using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditCards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "credit_card_accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Issuer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Brand = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LastFour = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OutstandingBalance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    AvailableCredit = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StatementClosingDay = table.Column<int>(type: "integer", nullable: false),
                    PaymentDueDay = table.Column<int>(type: "integer", nullable: false),
                    PaymentMode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RewardsProgram = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StatementDelivery = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    InterestNominalAnnual = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: true),
                    InterestEffectiveAnnual = table.Column<decimal>(type: "numeric(9,4)", precision: 9, scale: 4, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credit_card_accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_credit_card_accounts_accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "credit_card_statements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCardAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodStart = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StatementDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Purchases = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Fees = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Interest = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Payments = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StatementBalance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    MinimumPayment = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credit_card_statements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_credit_card_statements_credit_card_accounts_CreditCardAccou~",
                        column: x => x.CreditCardAccountId,
                        principalTable: "credit_card_accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "credit_card_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditCardAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    OperationType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    StatementId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsForeign = table.Column<bool>(type: "boolean", nullable: false),
                    InstallmentCount = table.Column<int>(type: "integer", nullable: true),
                    Merchant = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credit_card_transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_credit_card_transactions_credit_card_accounts_CreditCardAcc~",
                        column: x => x.CreditCardAccountId,
                        principalTable: "credit_card_accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_credit_card_transactions_credit_card_statements_StatementId",
                        column: x => x.StatementId,
                        principalTable: "credit_card_statements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_credit_card_transactions_transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_accounts_AccountId",
                table: "credit_card_accounts",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_accounts_IsActive",
                table: "credit_card_accounts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_accounts_UserId",
                table: "credit_card_accounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_statements_CreditCardAccountId",
                table: "credit_card_statements",
                column: "CreditCardAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_statements_DueDate",
                table: "credit_card_statements",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_statements_Status",
                table: "credit_card_statements",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_statements_UserId",
                table: "credit_card_statements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_transactions_CreditCardAccountId",
                table: "credit_card_transactions",
                column: "CreditCardAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_transactions_OperationType",
                table: "credit_card_transactions",
                column: "OperationType");

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_transactions_StatementId",
                table: "credit_card_transactions",
                column: "StatementId");

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_transactions_TransactionId",
                table: "credit_card_transactions",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_credit_card_transactions_UserId",
                table: "credit_card_transactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "credit_card_transactions");

            migrationBuilder.DropTable(
                name: "credit_card_statements");

            migrationBuilder.DropTable(
                name: "credit_card_accounts");
        }
    }
}
