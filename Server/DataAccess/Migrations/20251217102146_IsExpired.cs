using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class IsExpired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Repeatings",
                table: "LotteryTickets");

            migrationBuilder.AddColumn<bool>(
                name: "IsExpired",
                table: "LotteryTickets",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExpired",
                table: "LotteryTickets");

            migrationBuilder.AddColumn<int>(
                name: "Repeatings",
                table: "LotteryTickets",
                type: "integer",
                nullable: true);
        }
    }
}
