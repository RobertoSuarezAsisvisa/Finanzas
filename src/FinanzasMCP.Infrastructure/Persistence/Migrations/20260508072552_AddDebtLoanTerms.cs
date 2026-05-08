using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDebtLoanTerms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AmortizationMethod",
                table: "debts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InterestPeriod",
                table: "debts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRate",
                table: "debts",
                type: "numeric(9,4)",
                precision: 9,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LoanStartDate",
                table: "debts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TermMonths",
                table: "debts",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmortizationMethod",
                table: "debts");

            migrationBuilder.DropColumn(
                name: "InterestPeriod",
                table: "debts");

            migrationBuilder.DropColumn(
                name: "InterestRate",
                table: "debts");

            migrationBuilder.DropColumn(
                name: "LoanStartDate",
                table: "debts");

            migrationBuilder.DropColumn(
                name: "TermMonths",
                table: "debts");
        }
    }
}
