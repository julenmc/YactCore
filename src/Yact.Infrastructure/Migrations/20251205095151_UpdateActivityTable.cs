using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActivityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsIndoor",
                table: "Activities");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "EndDate", "StartDate", "Type" },
                values: new object[] { new DateTime(2025, 12, 5, 10, 51, 50, 751, DateTimeKind.Local).AddTicks(4968), new DateTime(2025, 12, 5, 10, 51, 50, 751, DateTimeKind.Local).AddTicks(4965), new DateTime(2025, 12, 5, 10, 21, 50, 751, DateTimeKind.Local).AddTicks(4918), "Cycling" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Activities");

            migrationBuilder.AddColumn<bool>(
                name: "IsIndoor",
                table: "Activities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "EndDate", "IsIndoor", "StartDate" },
                values: new object[] { new DateTime(2025, 12, 3, 16, 18, 22, 825, DateTimeKind.Local).AddTicks(3230), new DateTime(2025, 12, 3, 16, 18, 22, 825, DateTimeKind.Local).AddTicks(3228), false, new DateTime(2025, 12, 3, 15, 48, 22, 825, DateTimeKind.Local).AddTicks(3176) });
        }
    }
}
