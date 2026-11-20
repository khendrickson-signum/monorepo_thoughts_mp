using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EdisonProjectDatabase.Migrations
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
                    { new Guid("df5d2ef5-8d86-4944-badb-95e55e8ac32a"), true, "Edison #2", new Guid("f7b0b4b3-9efb-4508-9b6b-fcfebd121ee5") },
                    { new Guid("e936aa54-4fee-431c-9d1d-c4ee9cc5d751"), true, "Edison #1", new Guid("2e8f0a99-e7c0-4c30-bea2-6b242ea28084") }
                });

            migrationBuilder.InsertData(
                table: "MachineConfigsThisCustomerSupports",
                columns: new[] { "Id", "DeviceId", "MachineTypeId", "Name", "SerialNumber" },
                values: new object[,]
                {
                    { new Guid("41233157-3161-457e-85a9-ee7fd85d01c6"), new Guid("df5d2ef5-8d86-4944-badb-95e55e8ac32a"), new Guid("246cc725-457f-4627-aab2-9bffb91ead71"), "Edison Cow Milker", null },
                    { new Guid("efb30937-550b-44c7-baea-30cfe42640fe"), new Guid("e936aa54-4fee-431c-9d1d-c4ee9cc5d751"), new Guid("f847285a-da02-4cd2-a80c-ccadc930789b"), "Edison Grain Bin Reader", null }
                });

            migrationBuilder.InsertData(
                table: "MachineTypesForThisCustomer",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("246cc725-457f-4627-aab2-9bffb91ead71"), "Cow Milker" },
                    { new Guid("f847285a-da02-4cd2-a80c-ccadc930789b"), "Grain Bin Reader" }
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
