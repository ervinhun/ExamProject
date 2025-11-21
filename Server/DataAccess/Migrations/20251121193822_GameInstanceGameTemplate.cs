using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class GameInstanceGameTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameTemplates",
                schema: "LotteryApp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpirationDayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    ExpirationTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    NumberRangeMin = table.Column<int>(type: "integer", nullable: false),
                    NumberRangeMax = table.Column<int>(type: "integer", nullable: false),
                    MaxNumbersPerBoard = table.Column<int>(type: "integer", nullable: false),
                    MinNumbersPerBoard = table.Column<int>(type: "integer", nullable: false),
                    BasePrice = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameInstances",
                schema: "LotteryApp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    Week = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsExpired = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameInstances_Admins_AdminId",
                        column: x => x.AdminId,
                        principalSchema: "LotteryApp",
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameInstances_GameTemplates_GameTemplateId",
                        column: x => x.GameTemplateId,
                        principalSchema: "LotteryApp",
                        principalTable: "GameTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WinningNumbers",
                schema: "LotteryApp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WinningNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WinningNumbers_GameInstances_GameInstanceId",
                        column: x => x.GameInstanceId,
                        principalSchema: "LotteryApp",
                        principalTable: "GameInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameInstances_AdminId",
                schema: "LotteryApp",
                table: "GameInstances",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_GameInstances_GameTemplateId",
                schema: "LotteryApp",
                table: "GameInstances",
                column: "GameTemplateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WinningNumbers_GameInstanceId",
                schema: "LotteryApp",
                table: "WinningNumbers",
                column: "GameInstanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WinningNumbers",
                schema: "LotteryApp");

            migrationBuilder.DropTable(
                name: "GameInstances",
                schema: "LotteryApp");

            migrationBuilder.DropTable(
                name: "GameTemplates",
                schema: "LotteryApp");
        }
    }
}
