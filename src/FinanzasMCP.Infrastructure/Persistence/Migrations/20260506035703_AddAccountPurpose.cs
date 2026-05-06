using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountPurpose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "accounts",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Spending");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "accounts");
        }
    }
}
