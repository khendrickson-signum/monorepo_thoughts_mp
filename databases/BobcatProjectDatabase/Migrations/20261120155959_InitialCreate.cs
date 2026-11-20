using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BobcatProjectDatabase.Migrations
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
                    { new Guid("34132d13-a970-4340-8e10-ed260ada6498"), true, "Bobcat #1", new Guid("5962a011-138c-4e50-9d22-7dd0bb6501be") },
                    { new Guid("c56ee274-baa2-4122-990b-8eb8655c5917"), true, "Bobcat #2", new Guid("c2165796-8976-4e4b-9719-958127a17518") }
                });

            migrationBuilder.InsertData(
                table: "MachineConfigsThisCustomerSupports",
                columns: new[] { "Id", "DeviceId", "MachineTypeId", "Name", "SerialNumber" },
                values: new object[,]
                {
                    { new Guid("789ae17b-a7fa-4c95-91a8-3b0ed4c60239"), new Guid("34132d13-a970-4340-8e10-ed260ada6498"), new Guid("192d2446-2643-4ac9-8038-1dc34f61dc7c"), "Bobcat Loader", null },
                    { new Guid("e772871c-ada9-4f50-b9c6-84ab793222df"), new Guid("c56ee274-baa2-4122-990b-8eb8655c5917"), new Guid("33e58b4c-8a8d-4eb4-bd90-3cbcb8d891e3"), "Bobcat Excavator", null }
                });

            migrationBuilder.InsertData(
                table: "MachineTypesForThisCustomer",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("192d2446-2643-4ac9-8038-1dc34f61dc7c"), "Loader" },
                    { new Guid("33e58b4c-8a8d-4eb4-bd90-3cbcb8d891e3"), "Excavator" }
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
