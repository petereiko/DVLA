using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModuleActionTable5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_AspNetUsers_ApplicationUserId1",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_ApplicationUserId1",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "AuditLogs");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "AuditLogs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ApplicationUserId",
                table: "AuditLogs",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_AspNetUsers_ApplicationUserId",
                table: "AuditLogs",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_AspNetUsers_ApplicationUserId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_ApplicationUserId",
                table: "AuditLogs");

            migrationBuilder.AlterColumn<long>(
                name: "ApplicationUserId",
                table: "AuditLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "AuditLogs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ApplicationUserId1",
                table: "AuditLogs",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_AspNetUsers_ApplicationUserId1",
                table: "AuditLogs",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
