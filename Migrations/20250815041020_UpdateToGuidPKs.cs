using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralAddressSystem.Migrations
{
    public partial class UpdateToGuidPKs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop FK constraints (only if they exist, using exact names from errors)
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__Addresses__Count__5535A963') ALTER TABLE [Addresses] DROP CONSTRAINT [FK__Addresses__Count__5535A963];");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__Provinces__Count__3B75D760') ALTER TABLE [Provinces] DROP CONSTRAINT [FK__Provinces__Count__3B75D760];");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__Addresses__Provi__5629CD9C') ALTER TABLE [Addresses] DROP CONSTRAINT [FK__Addresses__Provi__5629CD9C];");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__Districts__Provi__3F466844') ALTER TABLE [Districts] DROP CONSTRAINT [FK__Districts__Provi__3F466844];");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__Addresses__Distr__571DF1D5') ALTER TABLE [Addresses] DROP CONSTRAINT [FK__Addresses__Distr__571DF1D5];");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Addresses_Districts_DistrictID') ALTER TABLE [Addresses] DROP CONSTRAINT [FK_Addresses_Districts_DistrictID];");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Addresses_States_StateID') ALTER TABLE [Addresses] DROP CONSTRAINT [FK_Addresses_States_StateID];");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Addresses_ZipCodes_ZipCodeZipID') ALTER TABLE [Addresses] DROP CONSTRAINT [FK_Addresses_ZipCodes_ZipCodeZipID];");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ZipCodes_States_StateID') ALTER TABLE [ZipCodes] DROP CONSTRAINT [FK_ZipCodes_States_StateID];");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Addresses_AspNetUsers_UserID') ALTER TABLE [Addresses] DROP CONSTRAINT [FK_Addresses_AspNetUsers_UserID];");

            // Step 2: Drop PKs (only if they exist, using exact names where known)
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK__Countrie__10D160BFED6F90B1') ALTER TABLE [Countries] DROP CONSTRAINT [PK__Countrie__10D160BFED6F90B1];");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK__Province__FD0A6FA352C46EC7') ALTER TABLE [Provinces] DROP CONSTRAINT [PK__Province__FD0A6FA352C46EC7];");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK__District__85FDA4A60397FDFA') ALTER TABLE [Districts] DROP CONSTRAINT [PK__District__85FDA4A60397FDFA];");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK__Addresse__091C2A1B1A3F57A3') ALTER TABLE [Addresses] DROP CONSTRAINT [PK__Addresse__091C2A1B1A3F57A3];");

            // Step 3: Drop indexes for ZipCodes/States (if they exist)
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Addresses_StateID' AND object_id = OBJECT_ID('Addresses')) DROP INDEX IX_Addresses_StateID ON Addresses;");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Addresses_ZipCodeZipID' AND object_id = OBJECT_ID('Addresses')) DROP INDEX IX_Addresses_ZipCodeZipID ON Addresses;");

            // Step 4: Drop ZipCodes/States columns (if they exist)
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Addresses') AND name = 'StateID') ALTER TABLE Addresses DROP COLUMN StateID;");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Addresses') AND name = 'ZipCodeZipID') ALTER TABLE Addresses DROP COLUMN ZipCodeZipID;");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Addresses') AND name = 'ZipID') ALTER TABLE Addresses DROP COLUMN ZipID;");

            // Step 5: Drop ZipCodes/States tables (if they exist)
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ZipCodes') DROP TABLE ZipCodes;");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.tables WHERE name = 'States') DROP TABLE States;");

            // Step 6: Add temp Guid columns for new PKs
            migrationBuilder.Sql("ALTER TABLE Countries ADD CountryID_Temp uniqueidentifier NOT NULL DEFAULT NEWID();");
            migrationBuilder.Sql("ALTER TABLE Provinces ADD ProvinceID_Temp uniqueidentifier NOT NULL DEFAULT NEWID();");
            migrationBuilder.Sql("ALTER TABLE Districts ADD DistrictID_Temp uniqueidentifier NOT NULL DEFAULT NEWID();");
            migrationBuilder.Sql("ALTER TABLE Addresses ADD AddressID_Temp uniqueidentifier NOT NULL DEFAULT NEWID();");

            // Step 7: Add temp Guid FK columns
            migrationBuilder.Sql("ALTER TABLE Provinces ADD CountryID_Temp uniqueidentifier;");
            migrationBuilder.Sql("ALTER TABLE Districts ADD ProvinceID_Temp uniqueidentifier;");
            migrationBuilder.Sql("ALTER TABLE Addresses ADD CountryID_Temp uniqueidentifier;");
            migrationBuilder.Sql("ALTER TABLE Addresses ADD ProvinceID_Temp uniqueidentifier;");
            migrationBuilder.Sql("ALTER TABLE Addresses ADD DistrictID_Temp uniqueidentifier;");
            migrationBuilder.Sql("ALTER TABLE Addresses ADD LocalBodyID_Temp uniqueidentifier;");

            // Step 8: Update temp FKs by joining on old IDs
            migrationBuilder.Sql(@"
                UPDATE p
                SET p.CountryID_Temp = c.CountryID_Temp
                FROM Provinces p
                INNER JOIN Countries c ON p.CountryID = c.CountryID;
            ");
            migrationBuilder.Sql(@"
                UPDATE d
                SET d.ProvinceID_Temp = p.ProvinceID_Temp
                FROM Districts d
                INNER JOIN Provinces p ON d.ProvinceID = p.ProvinceID;
            ");
            migrationBuilder.Sql(@"
                UPDATE a
                SET a.CountryID_Temp = c.CountryID_Temp
                FROM Addresses a
                INNER JOIN Countries c ON a.CountryID = c.CountryID;
            ");
            migrationBuilder.Sql(@"
                UPDATE a
                SET a.ProvinceID_Temp = p.ProvinceID_Temp
                FROM Addresses a
                LEFT JOIN Provinces p ON a.ProvinceID = p.ProvinceID;
            ");
            migrationBuilder.Sql(@"
                UPDATE a
                SET a.DistrictID_Temp = d.DistrictID_Temp
                FROM Addresses a
                LEFT JOIN Districts d ON a.DistrictID = d.DistrictID;
            ");

            // Step 9: Update Street column length
            migrationBuilder.Sql("ALTER TABLE Addresses ALTER COLUMN Street nvarchar(200) NOT NULL;");

            // Step 10: Drop old columns
            migrationBuilder.Sql("ALTER TABLE Countries DROP COLUMN CountryID;");
            migrationBuilder.Sql("ALTER TABLE Provinces DROP COLUMN ProvinceID;");
            migrationBuilder.Sql("ALTER TABLE Provinces DROP COLUMN CountryID;");
            migrationBuilder.Sql("ALTER TABLE Districts DROP COLUMN DistrictID;");
            migrationBuilder.Sql("ALTER TABLE Districts DROP COLUMN ProvinceID;");
            migrationBuilder.Sql("ALTER TABLE Addresses DROP COLUMN AddressID;");
            migrationBuilder.Sql("ALTER TABLE Addresses DROP COLUMN CountryID;");
            migrationBuilder.Sql("ALTER TABLE Addresses DROP COLUMN ProvinceID;");
            migrationBuilder.Sql("ALTER TABLE Addresses DROP COLUMN DistrictID;");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Addresses') AND name = 'LocalBodyID') ALTER TABLE Addresses DROP COLUMN LocalBodyID;");

            // Step 11: Rename temp columns
            migrationBuilder.Sql("EXEC sp_rename 'Countries.CountryID_Temp', 'CountryID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Provinces.ProvinceID_Temp', 'ProvinceID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Provinces.CountryID_Temp', 'CountryID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Districts.DistrictID_Temp', 'DistrictID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Districts.ProvinceID_Temp', 'ProvinceID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Addresses.AddressID_Temp', 'AddressID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Addresses.CountryID_Temp', 'CountryID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Addresses.ProvinceID_Temp', 'ProvinceID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Addresses.DistrictID_Temp', 'DistrictID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Addresses.LocalBodyID_Temp', 'LocalBodyID', 'COLUMN';");

            // Step 12: Recreate PKs
            migrationBuilder.AddPrimaryKey("PK_Countries", "Countries", "CountryID");
            migrationBuilder.AddPrimaryKey("PK_Provinces", "Provinces", "ProvinceID");
            migrationBuilder.AddPrimaryKey("PK_Districts", "Districts", "DistrictID");
            migrationBuilder.AddPrimaryKey("PK_Addresses", "Addresses", "AddressID");

            // Step 13: Create LocalBodies table
            migrationBuilder.CreateTable(
                name: "LocalBodies",
                columns: table => new
                {
                    LocalBodyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocalBodyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistrictID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalBodies", x => x.LocalBodyID);
                });

            // Step 14: Recreate FKs with adjusted cascade rules
            migrationBuilder.AddForeignKey("FK_Provinces_Countries_CountryID", "Provinces", "CountryID", "Countries", principalColumn: "CountryID", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_Districts_Provinces_ProvinceID", "Districts", "ProvinceID", "Provinces", principalColumn: "ProvinceID", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_LocalBodies_Districts_DistrictID", "LocalBodies", "DistrictID", "Districts", principalColumn: "DistrictID", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_Addresses_Countries_CountryID", "Addresses", "CountryID", "Countries", principalColumn: "CountryID", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_Addresses_Provinces_ProvinceID", "Addresses", "ProvinceID", "Provinces", principalColumn: "ProvinceID", onDelete: ReferentialAction.NoAction);
            migrationBuilder.AddForeignKey("FK_Addresses_Districts_DistrictID", "Addresses", "DistrictID", "Districts", principalColumn: "DistrictID", onDelete: ReferentialAction.NoAction);
            migrationBuilder.AddForeignKey("FK_Addresses_LocalBodies_LocalBodyID", "Addresses", "LocalBodyID", "LocalBodies", principalColumn: "LocalBodyID", onDelete: ReferentialAction.NoAction);
            migrationBuilder.AddForeignKey("FK_Addresses_AspNetUsers_UserID", "Addresses", "UserID", "AspNetUsers", principalColumn: "Id", onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop LocalBodies table
            migrationBuilder.DropTable(name: "LocalBodies");

            // Drop FKs
            migrationBuilder.DropForeignKey("FK_Provinces_Countries_CountryID", "Provinces");
            migrationBuilder.DropForeignKey("FK_Districts_Provinces_ProvinceID", "Districts");
            migrationBuilder.DropForeignKey("FK_Addresses_Countries_CountryID", "Addresses");
            migrationBuilder.DropForeignKey("FK_Addresses_Provinces_ProvinceID", "Addresses");
            migrationBuilder.DropForeignKey("FK_Addresses_Districts_DistrictID", "Addresses");
            migrationBuilder.DropForeignKey("FK_Addresses_LocalBodies_LocalBodyID", "Addresses");
            migrationBuilder.DropForeignKey("FK_Addresses_AspNetUsers_UserID", "Addresses");

            // Drop PKs
            migrationBuilder.DropPrimaryKey("PK_Countries", "Countries");
            migrationBuilder.DropPrimaryKey("PK_Provinces", "Provinces");
            migrationBuilder.DropPrimaryKey("PK_Districts", "Districts");
            migrationBuilder.DropPrimaryKey("PK_Addresses", "Addresses");

            // Add old int columns
            migrationBuilder.Sql("ALTER TABLE Countries ADD CountryID_Temp int NOT NULL IDENTITY(1,1);");
            migrationBuilder.Sql("ALTER TABLE Provinces ADD ProvinceID_Temp int NOT NULL IDENTITY(1,1);");
            migrationBuilder.Sql("ALTER TABLE Provinces ADD CountryID_Temp int NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE Districts ADD DistrictID_Temp int NOT NULL IDENTITY(1,1);");
            migrationBuilder.Sql("ALTER TABLE Districts ADD ProvinceID_Temp int NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE Addresses ADD CountryID_Temp int NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE Addresses ADD ProvinceID_Temp int NULL;");
            migrationBuilder.Sql("ALTER TABLE Addresses ADD DistrictID_Temp int NULL;");

            // Update temp int FKs (simplified; data loss possible)
            migrationBuilder.Sql(@"
                UPDATE p
                SET p.CountryID_Temp = c.CountryID_Temp
                FROM Provinces p
                INNER JOIN Countries c ON p.CountryID = c.CountryID;
            ");
            migrationBuilder.Sql(@"
                UPDATE d
                SET d.ProvinceID_Temp = p.ProvinceID_Temp
                FROM Districts d
                INNER JOIN Provinces p ON d.ProvinceID = p.ProvinceID;
            ");
            migrationBuilder.Sql(@"
                UPDATE a
                SET a.CountryID_Temp = c.CountryID_Temp
                FROM Addresses a
                INNER JOIN Countries c ON a.CountryID = c.CountryID;
            ");
            migrationBuilder.Sql(@"
                UPDATE a
                SET a.ProvinceID_Temp = p.ProvinceID_Temp
                FROM Addresses a
                LEFT JOIN Provinces p ON a.ProvinceID = p.ProvinceID;
            ");
            migrationBuilder.Sql(@"
                UPDATE a
                SET a.DistrictID_Temp = d.DistrictID_Temp
                FROM Addresses a
                LEFT JOIN Districts d ON a.DistrictID = d.DistrictID;
            ");

            // Revert Street column length
            migrationBuilder.Sql("ALTER TABLE Addresses ALTER COLUMN Street nvarchar(max) NOT NULL;");

            // Drop Guid columns
            migrationBuilder.Sql("ALTER TABLE Countries DROP COLUMN CountryID;");
            migrationBuilder.Sql("ALTER TABLE Provinces DROP COLUMN ProvinceID;");
            migrationBuilder.Sql("ALTER TABLE Provinces DROP COLUMN CountryID;");
            migrationBuilder.Sql("ALTER TABLE Districts DROP COLUMN DistrictID;");
            migrationBuilder.Sql("ALTER TABLE Districts DROP COLUMN ProvinceID;");
            migrationBuilder.Sql("ALTER TABLE Addresses DROP COLUMN CountryID;");
            migrationBuilder.Sql("ALTER TABLE Addresses DROP COLUMN ProvinceID;");
            migrationBuilder.Sql("ALTER TABLE Addresses DROP COLUMN DistrictID;");
            migrationBuilder.Sql("ALTER TABLE Addresses DROP COLUMN LocalBodyID;");

            // Rename temp int columns
            migrationBuilder.Sql("EXEC sp_rename 'Countries.CountryID_Temp', 'CountryID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Provinces.ProvinceID_Temp', 'ProvinceID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Provinces.CountryID_Temp', 'CountryID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Districts.DistrictID_Temp', 'DistrictID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Districts.ProvinceID_Temp', 'ProvinceID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Addresses.CountryID_Temp', 'CountryID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Addresses.ProvinceID_Temp', 'ProvinceID', 'COLUMN';");
            migrationBuilder.Sql("EXEC sp_rename 'Addresses.DistrictID_Temp', 'DistrictID', 'COLUMN';");

            // Recreate PKs
            migrationBuilder.AddPrimaryKey("PK_Countries", "Countries", "CountryID");
            migrationBuilder.AddPrimaryKey("PK_Provinces", "Provinces", "ProvinceID");
            migrationBuilder.AddPrimaryKey("PK_Districts", "Districts", "DistrictID");
            migrationBuilder.AddPrimaryKey("PK_Addresses", "Addresses", "AddressID");

            // Recreate FKs
            migrationBuilder.AddForeignKey("FK_Provinces_Countries_CountryID", "Provinces", "CountryID", "Countries", principalColumn: "CountryID", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_Districts_Provinces_ProvinceID", "Districts", "ProvinceID", "Provinces", principalColumn: "ProvinceID", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_Addresses_Countries_CountryID", "Addresses", "CountryID", "Countries", principalColumn: "CountryID", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey("FK_Addresses_Provinces_ProvinceID", "Addresses", "ProvinceID", "Provinces", principalColumn: "ProvinceID");
            migrationBuilder.AddForeignKey("FK_Addresses_Districts_DistrictID", "Addresses", "DistrictID", "Districts", principalColumn: "DistrictID");
            migrationBuilder.AddForeignKey("FK_Addresses_AspNetUsers_UserID", "Addresses", "UserID", "AspNetUsers", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
        }
    }
}