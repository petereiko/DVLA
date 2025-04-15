using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.VerificationPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApiClientTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DVLAReferenceNo",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "DriversLicence",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "FormNumber",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "LearnerDriversLicence",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "TaxIdentificationNumber",
                table: "VisualAssessmentResults");

            migrationBuilder.RenameColumn(
                name: "NameTitle",
                table: "VisualAssessmentResults",
                newName: "Gender");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransmittedDate",
                table: "VisualAssessmentResults",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "VisualAssessmentResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptometristFirmName",
                table: "VisualAssessmentResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptometristName",
                table: "VisualAssessmentResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CentreName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApiClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApiKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiClients", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiClients");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "OptometristFirmName",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "OptometristName",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "CentreName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "VisualAssessmentResults",
                newName: "NameTitle");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransmittedDate",
                table: "VisualAssessmentResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DVLAReferenceNo",
                table: "VisualAssessmentResults",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriversLicence",
                table: "VisualAssessmentResults",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormNumber",
                table: "VisualAssessmentResults",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LearnerDriversLicence",
                table: "VisualAssessmentResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxIdentificationNumber",
                table: "VisualAssessmentResults",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
