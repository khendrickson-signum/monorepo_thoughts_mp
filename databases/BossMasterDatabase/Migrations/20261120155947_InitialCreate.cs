using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BossMasterDatabase.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllSignumDevicesAcrossAllCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllSignumDevicesAcrossAllCustomers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AllSignumDevicesAcrossAllCustomers",
                columns: new[] { "Id", "IsActive", "Name", "ProjectId", "SerialNumber" },
                values: new object[,]
                {
                    { new Guid("1e5fc019-2561-471d-8622-357bd8675d9c"), true, "Signum Ixom IOT Device 1", new Guid("390a9eeb-470b-4dec-a0a0-ac3f4e9e9d2b"), "IXOM-IOT-1" },
                    { new Guid("2e8f0a99-e7c0-4c30-bea2-6b242ea28084"), true, "Signum Edison IOT Device 1", new Guid("b6719597-addc-4a69-882c-fc2ed1b486e0"), "EDISON-IOT-1" },
                    { new Guid("5962a011-138c-4e50-9d22-7dd0bb6501be"), true, "Signum Bobcat IOT Device 1", new Guid("19c08948-2dcc-4076-a27a-24535a092b86"), "BOBCAT-IOT-1" },
                    { new Guid("c2165796-8976-4e4b-9719-958127a17518"), true, "Signum Bobcat IOT Device 2", new Guid("19c08948-2dcc-4076-a27a-24535a092b86"), "BOBCAT-IOT-2" },
                    { new Guid("d1e12a24-a49e-41de-b4b6-0c87b67521ca"), true, "Signum Ixom IOT Device 2", new Guid("390a9eeb-470b-4dec-a0a0-ac3f4e9e9d2b"), "IXOM-IOT-2" },
                    { new Guid("f7b0b4b3-9efb-4508-9b6b-fcfebd121ee5"), true, "Signum Edison IOT Device 2", new Guid("b6719597-addc-4a69-882c-fc2ed1b486e0"), "EDISON-IOT-2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllSignumDevicesAcrossAllCustomers");
        }
    }
}
