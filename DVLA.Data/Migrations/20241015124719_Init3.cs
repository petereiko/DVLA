using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Districts_Regions_RegionId1",
                table: "Districts");

            migrationBuilder.DropIndex(
                name: "IX_Districts_RegionId1",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "RegionId1",
                table: "Districts");

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "Districts",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_RegionId",
                table: "Districts",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Districts_Regions_RegionId",
                table: "Districts",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Districts_Regions_RegionId",
                table: "Districts");

            migrationBuilder.DropIndex(
                name: "IX_Districts_RegionId",
                table: "Districts");

            migrationBuilder.AlterColumn<long>(
                name: "RegionId",
                table: "Districts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RegionId1",
                table: "Districts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Districts_RegionId1",
                table: "Districts",
                column: "RegionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Districts_Regions_RegionId1",
                table: "Districts",
                column: "RegionId1",
                principalTable: "Regions",
                principalColumn: "Id");
        }
    }
}
