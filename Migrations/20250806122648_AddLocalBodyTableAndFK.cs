using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralAddressSystem.Migrations
{
    public partial class AddLocalBodyTableAndFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            // 2. Recreate the LocalBodies table with Guid PK
            migrationBuilder.CreateTable(
                name: "LocalBodies",
                columns: table => new
                {
                    LocalBodyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocalBodyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistrictID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalBodies", x => x.LocalBodyID);
                    table.ForeignKey(
                        name: "FK_LocalBodies_Districts_DistrictID",
                        column: x => x.DistrictID,
                        principalTable: "Districts",
                        principalColumn: "DistrictID",
                        onDelete: ReferentialAction.Cascade);
                });

            // 3. Alter Addresses.LocalBodyID to Guid
            migrationBuilder.AlterColumn<Guid>(
                name: "LocalBodyID",
                table: "Addresses",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 4. Add FK from Addresses → LocalBodies
            migrationBuilder.CreateIndex(
                name: "IX_Addresses_LocalBodyID",
                table: "Addresses",
                column: "LocalBodyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_LocalBodies_LocalBodyID",
                table: "Addresses",
                column: "LocalBodyID",
                principalTable: "LocalBodies",
                principalColumn: "LocalBodyID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Remove FK from Addresses → LocalBodies
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_LocalBodies_LocalBodyID",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_LocalBodyID",
                table: "Addresses");

            // 2. Drop and recreate LocalBodies as int IDENTITY
            migrationBuilder.DropTable(
                name: "LocalBodies");

            migrationBuilder.CreateTable(
                name: "LocalBodies",
                columns: table => new
                {
                    LocalBodyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocalBodyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistrictID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalBodies", x => x.LocalBodyID);
                    table.ForeignKey(
                        name: "FK_LocalBodies_Districts_DistrictID",
                        column: x => x.DistrictID,
                        principalTable: "Districts",
                        principalColumn: "DistrictID",
                        onDelete: ReferentialAction.Cascade);
                });

            // 3. Alter Addresses.LocalBodyID back to int
            migrationBuilder.AlterColumn<int>(
                name: "LocalBodyID",
                table: "Addresses",
                type: "int",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
