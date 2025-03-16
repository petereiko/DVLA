using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.VerificationPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IsVerifiedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "VisualAssessmentResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedDate",
                table: "VisualAssessmentResults",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "VisualAssessmentResults");

            migrationBuilder.DropColumn(
                name: "VerifiedDate",
                table: "VisualAssessmentResults");
        }
    }
}
