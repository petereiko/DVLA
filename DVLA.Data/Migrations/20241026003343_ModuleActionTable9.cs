using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModuleActionTable9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InitiatePaystackTransferRequestId",
                table: "InitiatePaystackTransferResponses",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InitiatePaystackTransferResponses_InitiatePaystackTransferRequestId",
                table: "InitiatePaystackTransferResponses",
                column: "InitiatePaystackTransferRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_InitiatePaystackTransferResponses_InitiatePaystackTransferRequests_InitiatePaystackTransferRequestId",
                table: "InitiatePaystackTransferResponses",
                column: "InitiatePaystackTransferRequestId",
                principalTable: "InitiatePaystackTransferRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InitiatePaystackTransferResponses_InitiatePaystackTransferRequests_InitiatePaystackTransferRequestId",
                table: "InitiatePaystackTransferResponses");

            migrationBuilder.DropIndex(
                name: "IX_InitiatePaystackTransferResponses_InitiatePaystackTransferRequestId",
                table: "InitiatePaystackTransferResponses");

            migrationBuilder.DropColumn(
                name: "InitiatePaystackTransferRequestId",
                table: "InitiatePaystackTransferResponses");
        }
    }
}
