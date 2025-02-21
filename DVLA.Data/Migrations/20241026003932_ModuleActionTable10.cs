using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVLA.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModuleActionTable10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "SlotRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "SlotRequests");
        }
    }
}
