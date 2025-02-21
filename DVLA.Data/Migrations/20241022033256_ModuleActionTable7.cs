using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModuleActionTable7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsTemplateTokens_SmsTemplates_SmsTemplateId1",
                table: "SmsTemplateTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsTemplateTokens_SmsTokens_SmsTokenId1",
                table: "SmsTemplateTokens");

            migrationBuilder.DropIndex(
                name: "IX_SmsTemplateTokens_SmsTemplateId1",
                table: "SmsTemplateTokens");

            migrationBuilder.DropIndex(
                name: "IX_SmsTemplateTokens_SmsTokenId1",
                table: "SmsTemplateTokens");

            migrationBuilder.DropColumn(
                name: "SmsTemplateId1",
                table: "SmsTemplateTokens");

            migrationBuilder.DropColumn(
                name: "SmsTokenId1",
                table: "SmsTemplateTokens");

            migrationBuilder.AlterColumn<int>(
                name: "SmsTokenId",
                table: "SmsTemplateTokens",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "SmsTemplateId",
                table: "SmsTemplateTokens",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_SmsTemplateTokens_SmsTemplateId",
                table: "SmsTemplateTokens",
                column: "SmsTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsTemplateTokens_SmsTokenId",
                table: "SmsTemplateTokens",
                column: "SmsTokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsTemplateTokens_SmsTemplates_SmsTemplateId",
                table: "SmsTemplateTokens",
                column: "SmsTemplateId",
                principalTable: "SmsTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsTemplateTokens_SmsTokens_SmsTokenId",
                table: "SmsTemplateTokens",
                column: "SmsTokenId",
                principalTable: "SmsTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsTemplateTokens_SmsTemplates_SmsTemplateId",
                table: "SmsTemplateTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsTemplateTokens_SmsTokens_SmsTokenId",
                table: "SmsTemplateTokens");

            migrationBuilder.DropIndex(
                name: "IX_SmsTemplateTokens_SmsTemplateId",
                table: "SmsTemplateTokens");

            migrationBuilder.DropIndex(
                name: "IX_SmsTemplateTokens_SmsTokenId",
                table: "SmsTemplateTokens");

            migrationBuilder.AlterColumn<long>(
                name: "SmsTokenId",
                table: "SmsTemplateTokens",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "SmsTemplateId",
                table: "SmsTemplateTokens",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SmsTemplateId1",
                table: "SmsTemplateTokens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SmsTokenId1",
                table: "SmsTemplateTokens",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmsTemplateTokens_SmsTemplateId1",
                table: "SmsTemplateTokens",
                column: "SmsTemplateId1");

            migrationBuilder.CreateIndex(
                name: "IX_SmsTemplateTokens_SmsTokenId1",
                table: "SmsTemplateTokens",
                column: "SmsTokenId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsTemplateTokens_SmsTemplates_SmsTemplateId1",
                table: "SmsTemplateTokens",
                column: "SmsTemplateId1",
                principalTable: "SmsTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsTemplateTokens_SmsTokens_SmsTokenId1",
                table: "SmsTemplateTokens",
                column: "SmsTokenId1",
                principalTable: "SmsTokens",
                principalColumn: "Id");
        }
    }
}
