using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "PrefixName",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Regions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Regions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Regions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Regions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PrefixName",
                table: "Regions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Regions",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }
    }
}
