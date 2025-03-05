using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.VerificationPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VisualAssessmentResultAlter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "IsSynchronized",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "IsTransmitted",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "OldDVLAReferenceNo",
                table: "VisualAssessmentResults");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "VisualAssessmentResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "VisualAssessmentResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "VisualAssessmentResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "VisualAssessmentResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSynchronized",
                table: "VisualAssessmentResults",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTransmitted",
                table: "VisualAssessmentResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "VisualAssessmentResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "VisualAssessmentResults",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldDVLAReferenceNo",
                table: "VisualAssessmentResults",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
