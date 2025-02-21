using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModuleActionTable6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplateTokens_EmailTemplates_EmailTemplateId1",
                table: "EmailTemplateTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplateTokens_EmailTokens_EmailTokenId1",
                table: "EmailTemplateTokens");

            migrationBuilder.DropIndex(
                name: "IX_EmailTemplateTokens_EmailTemplateId1",
                table: "EmailTemplateTokens");

            migrationBuilder.DropIndex(
                name: "IX_EmailTemplateTokens_EmailTokenId1",
                table: "EmailTemplateTokens");

            migrationBuilder.DropColumn(
                name: "EmailTemplateId1",
                table: "EmailTemplateTokens");

            migrationBuilder.DropColumn(
                name: "EmailTokenId1",
                table: "EmailTemplateTokens");

            migrationBuilder.AlterColumn<int>(
                name: "EmailTokenId",
                table: "EmailTemplateTokens",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "EmailTemplateId",
                table: "EmailTemplateTokens",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplateTokens_EmailTemplateId",
                table: "EmailTemplateTokens",
                column: "EmailTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplateTokens_EmailTokenId",
                table: "EmailTemplateTokens",
                column: "EmailTokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplateTokens_EmailTemplates_EmailTemplateId",
                table: "EmailTemplateTokens",
                column: "EmailTemplateId",
                principalTable: "EmailTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplateTokens_EmailTokens_EmailTokenId",
                table: "EmailTemplateTokens",
                column: "EmailTokenId",
                principalTable: "EmailTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplateTokens_EmailTemplates_EmailTemplateId",
                table: "EmailTemplateTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplateTokens_EmailTokens_EmailTokenId",
                table: "EmailTemplateTokens");

            migrationBuilder.DropIndex(
                name: "IX_EmailTemplateTokens_EmailTemplateId",
                table: "EmailTemplateTokens");

            migrationBuilder.DropIndex(
                name: "IX_EmailTemplateTokens_EmailTokenId",
                table: "EmailTemplateTokens");

            migrationBuilder.AlterColumn<long>(
                name: "EmailTokenId",
                table: "EmailTemplateTokens",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "EmailTemplateId",
                table: "EmailTemplateTokens",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "EmailTemplateId1",
                table: "EmailTemplateTokens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmailTokenId1",
                table: "EmailTemplateTokens",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplateTokens_EmailTemplateId1",
                table: "EmailTemplateTokens",
                column: "EmailTemplateId1");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplateTokens_EmailTokenId1",
                table: "EmailTemplateTokens",
                column: "EmailTokenId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplateTokens_EmailTemplates_EmailTemplateId1",
                table: "EmailTemplateTokens",
                column: "EmailTemplateId1",
                principalTable: "EmailTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplateTokens_EmailTokens_EmailTokenId1",
                table: "EmailTemplateTokens",
                column: "EmailTokenId1",
                principalTable: "EmailTokens",
                principalColumn: "Id");
        }
    }
}
