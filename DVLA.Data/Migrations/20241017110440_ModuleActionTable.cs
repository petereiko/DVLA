using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModuleActionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Controler",
                table: "AuditLogs",
                newName: "Controller");

            migrationBuilder.AlterColumn<int>(
                name: "OptometristFirmId",
                table: "VisualAssessmentResults",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "PrefixName",
                table: "Regions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OptometristFirmId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormNumber",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRegistration",
                table: "Applicants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OldDVLAReferenceNo",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ModuleActions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleId = table.Column<long>(type: "bigint", nullable: false),
                    ModuleId1 = table.Column<int>(type: "int", nullable: true),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleActions_Modules_ModuleId1",
                        column: x => x.ModuleId1,
                        principalTable: "Modules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VisualAssessmentResults_OptometristFirmId",
                table: "VisualAssessmentResults",
                column: "OptometristFirmId");

            migrationBuilder.CreateIndex(
                name: "IX_OptometristFirmUsers_OptometristFirmId",
                table: "OptometristFirmUsers",
                column: "OptometristFirmId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleActions_ModuleId1",
                table: "ModuleActions",
                column: "ModuleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OptometristFirmUsers_OptometristFirms_OptometristFirmId",
                table: "OptometristFirmUsers",
                column: "OptometristFirmId",
                principalTable: "OptometristFirms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisualAssessmentResults_OptometristFirms_OptometristFirmId",
                table: "VisualAssessmentResults",
                column: "OptometristFirmId",
                principalTable: "OptometristFirms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OptometristFirmUsers_OptometristFirms_OptometristFirmId",
                table: "OptometristFirmUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_VisualAssessmentResults_OptometristFirms_OptometristFirmId",
                table: "VisualAssessmentResults");

            migrationBuilder.DropTable(
                name: "ModuleActions");

            migrationBuilder.DropIndex(
                name: "IX_VisualAssessmentResults_OptometristFirmId",
                table: "VisualAssessmentResults");

            migrationBuilder.DropIndex(
                name: "IX_OptometristFirmUsers_OptometristFirmId",
                table: "OptometristFirmUsers");

            migrationBuilder.DropColumn(
                name: "PrefixName",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "FormNumber",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "IsRegistration",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "OldDVLAReferenceNo",
                table: "Applicants");

            migrationBuilder.RenameColumn(
                name: "Controller",
                table: "AuditLogs",
                newName: "Controler");

            migrationBuilder.AlterColumn<long>(
                name: "OptometristFirmId",
                table: "VisualAssessmentResults",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "OptometristFirmId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
