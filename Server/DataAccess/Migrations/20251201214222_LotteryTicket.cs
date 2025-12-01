using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class LotteryTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LotteryTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullPrice = table.Column<double>(type: "double precision", nullable: false),
                    IsWinning = table.Column<bool>(type: "boolean", nullable: false),
                    BoughtAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotteryTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LotteryTickets_GameInstances_GameInstanceId",
                        column: x => x.GameInstanceId,
                        principalTable: "GameInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LotteryTickets_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PickedNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickedNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickedNumbers_LotteryTickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "LotteryTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotteryTickets_GameInstanceId",
                table: "LotteryTickets",
                column: "GameInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_LotteryTickets_PlayerId",
                table: "LotteryTickets",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PickedNumbers_TicketId",
                table: "PickedNumbers",
                column: "TicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickedNumbers");

            migrationBuilder.DropTable(
                name: "LotteryTickets");
        }
    }
}
