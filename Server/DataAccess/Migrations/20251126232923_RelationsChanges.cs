using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RelationsChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Players_PlayerId",
                schema: "LotteryApp",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Players_PlayerId",
                schema: "LotteryApp",
                table: "Transactions",
                column: "PlayerId",
                principalSchema: "LotteryApp",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Players_PlayerId",
                schema: "LotteryApp",
                table: "Transactions");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Players_PlayerId",
                schema: "LotteryApp",
                table: "Transactions",
                column: "PlayerId",
                principalSchema: "LotteryApp",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
