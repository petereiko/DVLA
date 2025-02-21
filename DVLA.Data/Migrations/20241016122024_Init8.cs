using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<int>(
                name: "OptometristFirmId",
                table: "Slots",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "OptometristFirmId",
                table: "SlotRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "OptometristFirmId",
                table: "OptometristFirmUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_OptometristFirmId",
                table: "Slots",
                column: "OptometristFirmId");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_OptometristFirms_OptometristFirmId",
                table: "Slots",
                column: "OptometristFirmId",
                principalTable: "OptometristFirms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_OptometristFirms_OptometristFirmId",
                table: "Slots");

            migrationBuilder.DropIndex(
                name: "IX_Slots_OptometristFirmId",
                table: "Slots");

            migrationBuilder.AlterColumn<long>(
                name: "OptometristFirmId",
                table: "Slots",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OptometristFirmId1",
                table: "Slots",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "OptometristFirmId",
                table: "SlotRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "OptometristFirmId",
                table: "OptometristFirmUsers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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
    }
}
