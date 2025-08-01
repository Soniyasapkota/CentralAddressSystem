using Microsoft.EntityFrameworkCore.Migrations;

namespace CentralAddressSystem.Migrations
{
    public partial class SeedNepalCountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Countries WHERE CountryID = 1)
                BEGIN
                    SET IDENTITY_INSERT [Countries] ON;
                    INSERT INTO [Countries] ([CountryID], [CountryCode], [CountryName], [CreatedAt])
                    VALUES (1, N'NP', N'Nepal', '2025-07-26 13:37:56');
                    SET IDENTITY_INSERT [Countries] OFF;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM Countries WHERE CountryID = 1)
                BEGIN
                    SET IDENTITY_INSERT [Countries] ON;
                    DELETE FROM [Countries] WHERE CountryID = 1;
                    SET IDENTITY_INSERT [Countries] OFF;
                END
            ");
        }
    }
}