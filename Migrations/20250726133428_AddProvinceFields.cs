using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralAddressSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddProvinceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Noofdistricts",
                table: "Provinces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceCode",
                table: "Provinces",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Noofdistricts",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "Provinces");
        }
    }
}
