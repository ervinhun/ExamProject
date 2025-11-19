using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTPT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Users_PlayerId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Users");

            migrationBuilder.EnsureSchema(
                name: "LotteryApp");

            migrationBuilder.RenameTable(
                name: "Wallets",
                newName: "Wallets",
                newSchema: "LotteryApp");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "LotteryApp");

            migrationBuilder.CreateTable(
                name: "Admins",
                schema: "LotteryApp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_Users_Id",
                        column: x => x.Id,
                        principalSchema: "LotteryApp",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                schema: "LotteryApp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Users_Id",
                        column: x => x.Id,
                        principalSchema: "LotteryApp",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Players_PlayerId",
                schema: "LotteryApp",
                table: "Wallets",
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
                name: "FK_Wallets_Players_PlayerId",
                schema: "LotteryApp",
                table: "Wallets");

            migrationBuilder.DropTable(
                name: "Admins",
                schema: "LotteryApp");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "LotteryApp");

            migrationBuilder.RenameTable(
                name: "Wallets",
                schema: "LotteryApp",
                newName: "Wallets");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "LotteryApp",
                newName: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Users",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Users_PlayerId",
                table: "Wallets",
                column: "PlayerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
