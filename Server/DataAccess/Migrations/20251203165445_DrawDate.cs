using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DrawDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "GameInstances");

            migrationBuilder.RenameColumn(
                name: "ExpirationTimeOfDay",
                table: "GameInstances",
                newName: "DrawTimeOfDay");

            migrationBuilder.RenameColumn(
                name: "ExpirationDayOfWeek",
                table: "GameInstances",
                newName: "DrawDayOfWeek");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DrawDate",
                table: "GameInstances",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DrawTimeOfDay",
                table: "GameInstances",
                newName: "ExpirationTimeOfDay");

            migrationBuilder.RenameColumn(
                name: "DrawDayOfWeek",
                table: "GameInstances",
                newName: "ExpirationDayOfWeek");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DrawDate",
                table: "GameInstances",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "GameInstances",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
