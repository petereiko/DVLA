using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OptometristFirms_Regions_RegionId",
                table: "OptometristFirms");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "OptometristFirmUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "OptometristFirms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DistrictId",
                table: "OptometristFirms",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_OptometristFirmUsers_ApplicationUserId",
                table: "OptometristFirmUsers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OptometristFirms_DistrictId",
                table: "OptometristFirms",
                column: "DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_OptometristFirms_Districts_DistrictId",
                table: "OptometristFirms",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OptometristFirms_Regions_RegionId",
                table: "OptometristFirms",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OptometristFirmUsers_AspNetUsers_ApplicationUserId",
                table: "OptometristFirmUsers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OptometristFirms_Districts_DistrictId",
                table: "OptometristFirms");

            migrationBuilder.DropForeignKey(
                name: "FK_OptometristFirms_Regions_RegionId",
                table: "OptometristFirms");

            migrationBuilder.DropForeignKey(
                name: "FK_OptometristFirmUsers_AspNetUsers_ApplicationUserId",
                table: "OptometristFirmUsers");

            migrationBuilder.DropIndex(
                name: "IX_OptometristFirmUsers_ApplicationUserId",
                table: "OptometristFirmUsers");

            migrationBuilder.DropIndex(
                name: "IX_OptometristFirms_DistrictId",
                table: "OptometristFirms");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "OptometristFirmUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "OptometristFirms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "DistrictId",
                table: "OptometristFirms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OptometristFirms_Regions_RegionId",
                table: "OptometristFirms",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
