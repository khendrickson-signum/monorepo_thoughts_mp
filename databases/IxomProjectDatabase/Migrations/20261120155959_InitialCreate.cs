using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IxomProjectDatabase.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DevicesForThisCustomer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignumGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevicesForThisCustomer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MachineConfigsThisCustomerSupports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MachineTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineConfigsThisCustomerSupports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MachineTypesForThisCustomer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineTypesForThisCustomer", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DevicesForThisCustomer",
                columns: new[] { "Id", "IsActive", "Name", "SignumGuid" },
                values: new object[,]
                {
                    { new Guid("2089940c-7c9c-47a5-a2d5-82221f5d3b9b"), true, "Ixom #1", new Guid("1e5fc019-2561-471d-8622-357bd8675d9c") },
                    { new Guid("571a9ea5-7fb0-4190-8639-2878d2dfac56"), true, "Ixom #2", new Guid("d1e12a24-a49e-41de-b4b6-0c87b67521ca") }
                });

            migrationBuilder.InsertData(
                table: "MachineConfigsThisCustomerSupports",
                columns: new[] { "Id", "DeviceId", "MachineTypeId", "Name", "SerialNumber" },
                values: new object[,]
                {
                    { new Guid("cb6358cd-ef64-4597-bbcb-70513d977a68"), new Guid("571a9ea5-7fb0-4190-8639-2878d2dfac56"), new Guid("c9dd93f0-36c4-42dd-b3ed-82d37ce23655"), "Ixom GS Mixer", null },
                    { new Guid("f0bec2f2-46e8-480a-9705-d80bf421d857"), new Guid("2089940c-7c9c-47a5-a2d5-82221f5d3b9b"), new Guid("efa04a71-1751-48e0-8d55-b8c46c0acf0f"), "Ixom SolarBee", null }
                });

            migrationBuilder.InsertData(
                table: "MachineTypesForThisCustomer",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("c9dd93f0-36c4-42dd-b3ed-82d37ce23655"), "GS Mixer" },
                    { new Guid("efa04a71-1751-48e0-8d55-b8c46c0acf0f"), "SolarBee" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DevicesForThisCustomer");

            migrationBuilder.DropTable(
                name: "MachineConfigsThisCustomerSupports");

            migrationBuilder.DropTable(
                name: "MachineTypesForThisCustomer");
        }
    }
}
