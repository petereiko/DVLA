using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OptometristFirmId1",
                table: "Slots",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SlotPrices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "SlotPrices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "SlotPrices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slots_OptometristFirmId1",
                table: "Slots",
                column: "OptometristFirmId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_OptometristFirms_OptometristFirmId1",
                table: "Slots",
                column: "OptometristFirmId1",
                principalTable: "OptometristFirms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_OptometristFirms_OptometristFirmId1",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_OptometristFirmId1",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "OptometristFirmId1",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SlotPrices");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "SlotPrices");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "SlotPrices");
        }
    }
}
