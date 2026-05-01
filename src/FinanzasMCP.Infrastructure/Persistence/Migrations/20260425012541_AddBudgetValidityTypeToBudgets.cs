using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetValidityTypeToBudgets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_budgets_CategoryId_PeriodType_PeriodStart",
                table: "budgets");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "PeriodStart",
                table: "budgets",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "PeriodEnd",
                table: "budgets",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "ValidityType",
                table: "budgets",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_budgets_CategoryId_PeriodType_ValidityType_PeriodStart",
                table: "budgets",
                columns: new[] { "CategoryId", "PeriodType", "ValidityType", "PeriodStart" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_budgets_CategoryId_PeriodType_ValidityType_PeriodStart",
                table: "budgets");

            migrationBuilder.DropColumn(
                name: "ValidityType",
                table: "budgets");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "PeriodStart",
                table: "budgets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "PeriodEnd",
                table: "budgets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_budgets_CategoryId_PeriodType_PeriodStart",
                table: "budgets",
                columns: new[] { "CategoryId", "PeriodType", "PeriodStart" },
                unique: true);
        }
    }
}
