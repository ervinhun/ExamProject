using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TicketSubs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketSubscriptionId",
                table: "PickedNumbers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TicketSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    IsExpired = table.Column<bool>(type: "boolean", nullable: false),
                    BoughtAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PickedNumbers_TicketSubscriptionId",
                table: "PickedNumbers",
                column: "TicketSubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PickedNumbers_TicketSubscriptions_TicketSubscriptionId",
                table: "PickedNumbers",
                column: "TicketSubscriptionId",
                principalTable: "TicketSubscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickedNumbers_TicketSubscriptions_TicketSubscriptionId",
                table: "PickedNumbers");

            migrationBuilder.DropTable(
                name: "TicketSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_PickedNumbers_TicketSubscriptionId",
                table: "PickedNumbers");

            migrationBuilder.DropColumn(
                name: "TicketSubscriptionId",
                table: "PickedNumbers");
        }
    }
}
