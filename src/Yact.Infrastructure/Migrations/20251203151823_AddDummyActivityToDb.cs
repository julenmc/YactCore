using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDummyActivityToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Elevation",
                table: "Activities",
                newName: "ElevationMeters");

            migrationBuilder.RenameColumn(
                name: "Distance",
                table: "Activities",
                newName: "DistanceMeters");

            migrationBuilder.InsertData(
                table: "Activities",
                columns: new[] { "Id", "CreateDate", "Description", "DistanceMeters", "ElevationMeters", "EndDate", "IsIndoor", "Name", "Path", "StartDate", "UpdateDate" },
                values: new object[] { 1, new DateTime(2025, 12, 3, 16, 18, 22, 825, DateTimeKind.Local).AddTicks(3230), "This is a dummy activity", 10000.0, 100.0, new DateTime(2025, 12, 3, 16, 18, 22, 825, DateTimeKind.Local).AddTicks(3228), false, "Dummy Activity", "dummy_activity.fit", new DateTime(2025, 12, 3, 15, 48, 22, 825, DateTimeKind.Local).AddTicks(3176), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "ElevationMeters",
                table: "Activities",
                newName: "Elevation");

            migrationBuilder.RenameColumn(
                name: "DistanceMeters",
                table: "Activities",
                newName: "Distance");
        }
    }
}
