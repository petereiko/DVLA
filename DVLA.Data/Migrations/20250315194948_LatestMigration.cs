using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class LatestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "DVLAReferenceNo",
            //    table: "VisualAssessmentResults");

            //migrationBuilder.DropColumn(
            //    name: "DriversLicence",
            //    table: "VisualAssessmentResults");

            //migrationBuilder.DropColumn(
            //    name: "FormNumber",
            //    table: "VisualAssessmentResults");

            //migrationBuilder.DropColumn(
            //    name: "OldDVLAReferenceNo",
            //    table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "DVLAReferenceNo",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "DriversLicence",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "FormNumber",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "NameTitle",
                table: "Applicants");

            //migrationBuilder.DropColumn(
            //    name: "OldDVLAReferenceNo",
            //    table: "Applicants");

            migrationBuilder.RenameColumn(
                name: "NameTitle",
                table: "VisualAssessmentResults",
                newName: "Gender");

            migrationBuilder.AddColumn<bool>(
                name: "IsTransmitted",
                table: "VisualAssessmentResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransmittedDate",
                table: "VisualAssessmentResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "VisualAssessmentTransmissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsTransmitted = table.Column<bool>(type: "bit", nullable: false),
                    TransmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordCount = table.Column<int>(type: "int", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisualAssessmentTransmissions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisualAssessmentTransmissions");

            migrationBuilder.DropColumn(
                name: "IsTransmitted",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "TransmittedDate",
                table: "VisualAssessmentResults");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "VisualAssessmentResults",
                newName: "NameTitle");

            //migrationBuilder.AddColumn<string>(
            //    name: "DVLAReferenceNo",
            //    table: "VisualAssessmentResults",
            //    type: "nvarchar(50)",
            //    maxLength: 50,
            //    nullable: true);

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

            //migrationBuilder.AddColumn<string>(
            //    name: "OldDVLAReferenceNo",
            //    table: "VisualAssessmentResults",
            //    type: "nvarchar(50)",
            //    maxLength: 50,
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DVLAReferenceNo",
                table: "Applicants",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriversLicence",
                table: "Applicants",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormNumber",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "Applicants",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NameTitle",
                table: "Applicants",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldDVLAReferenceNo",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
