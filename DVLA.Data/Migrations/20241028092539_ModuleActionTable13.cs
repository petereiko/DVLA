using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModuleActionTable13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SlotRequests_OptometristFirmId",
                table: "SlotRequests",
                column: "OptometristFirmId");

            migrationBuilder.AddForeignKey(
                name: "FK_SlotRequests_OptometristFirms_OptometristFirmId",
                table: "SlotRequests",
                column: "OptometristFirmId",
                principalTable: "OptometristFirms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SlotRequests_OptometristFirms_OptometristFirmId",
                table: "SlotRequests");

            migrationBuilder.DropIndex(
                name: "IX_SlotRequests_OptometristFirmId",
                table: "SlotRequests");
        }
    }
}
