using Microsoft.EntityFrameworkCore.Migrations;

namespace CentralAddressSystem.Migrations
{
    public partial class UpdateSchemaNoSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the unique constraint on CountryCode
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ__Countrie__5D9B0D2C11030DDB') ALTER TABLE [Countries] DROP CONSTRAINT [UQ__Countrie__5D9B0D2C11030DDB];");

            // Alter Province columns
            migrationBuilder.AlterColumn<string>(
                name: "ProvinceName",
                table: "Provinces",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProvinceCode",
                table: "Provinces",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Alter Country columns
            migrationBuilder.AlterColumn<string>(
                name: "CountryName",
                table: "Countries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Countries",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Re-add unique index on CountryCode (optional, include if uniqueness is required)
            migrationBuilder.CreateIndex(
                name: "IX_Countries_CountryCode",
                table: "Countries",
                column: "CountryCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop unique index (if added)
            migrationBuilder.DropIndex(
                name: "IX_Countries_CountryCode",
                table: "Countries");

            // Revert Country columns
            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "CountryName",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: false);

            // Revert Province columns
            migrationBuilder.AlterColumn<string>(
                name: "ProvinceCode",
                table: "Provinces",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "ProvinceName",
                table: "Provinces",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: false);

            // Re-add unique constraint
            migrationBuilder.Sql("ALTER TABLE [Countries] ADD CONSTRAINT [UQ__Countrie__5D9B0D2C11030DDB] UNIQUE ([CountryCode]);");
        }
    }
}