using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RolesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Players_PlayerId",
                schema: "LotteryApp",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Wallets_WalletId",
                schema: "LotteryApp",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                schema: "LotteryApp",
                table: "Transaction");

            migrationBuilder.RenameTable(
                name: "Transaction",
                schema: "LotteryApp",
                newName: "Transactions",
                newSchema: "LotteryApp");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_WalletId",
                schema: "LotteryApp",
                table: "Transactions",
                newName: "IX_Transactions_WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_PlayerId",
                schema: "LotteryApp",
                table: "Transactions",
                newName: "IX_Transactions_PlayerId");

            migrationBuilder.AlterColumn<double>(
                name: "Balance",
                schema: "LotteryApp",
                table: "Wallets",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshTokenHash",
                schema: "LotteryApp",
                table: "Users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "LotteryApp",
                table: "Users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                schema: "LotteryApp",
                table: "Transactions",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                schema: "LotteryApp",
                table: "Transactions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "LotteryApp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleUser",
                schema: "LotteryApp",
                columns: table => new
                {
                    RolesId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RoleUser_Roles_RolesId",
                        column: x => x.RolesId,
                        principalSchema: "LotteryApp",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalSchema: "LotteryApp",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UsersId",
                schema: "LotteryApp",
                table: "RoleUser",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Players_PlayerId",
                schema: "LotteryApp",
                table: "Transactions",
                column: "PlayerId",
                principalSchema: "LotteryApp",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Wallets_WalletId",
                schema: "LotteryApp",
                table: "Transactions",
                column: "WalletId",
                principalSchema: "LotteryApp",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Players_PlayerId",
                schema: "LotteryApp",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Wallets_WalletId",
                schema: "LotteryApp",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "RoleUser",
                schema: "LotteryApp");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "LotteryApp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                schema: "LotteryApp",
                table: "Transactions");

            migrationBuilder.RenameTable(
                name: "Transactions",
                schema: "LotteryApp",
                newName: "Transaction",
                newSchema: "LotteryApp");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_WalletId",
                schema: "LotteryApp",
                table: "Transaction",
                newName: "IX_Transaction_WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_PlayerId",
                schema: "LotteryApp",
                table: "Transaction",
                newName: "IX_Transaction_PlayerId");

            migrationBuilder.AlterColumn<long>(
                name: "Balance",
                schema: "LotteryApp",
                table: "Wallets",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshTokenHash",
                schema: "LotteryApp",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "LotteryApp",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "LotteryApp",
                table: "Transaction",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                schema: "LotteryApp",
                table: "Transaction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Players_PlayerId",
                schema: "LotteryApp",
                table: "Transaction",
                column: "PlayerId",
                principalSchema: "LotteryApp",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Wallets_WalletId",
                schema: "LotteryApp",
                table: "Transaction",
                column: "WalletId",
                principalSchema: "LotteryApp",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
