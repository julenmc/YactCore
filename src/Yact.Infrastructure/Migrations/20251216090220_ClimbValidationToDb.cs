using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClimbValidationToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Validated",
                table: "Climbs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ActivityInfos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2025, 12, 16, 10, 2, 20, 140, DateTimeKind.Local).AddTicks(2310), new DateTime(2025, 12, 16, 10, 2, 20, 140, DateTimeKind.Local).AddTicks(2307), new DateTime(2025, 12, 16, 9, 32, 20, 140, DateTimeKind.Local).AddTicks(2303) });

            migrationBuilder.UpdateData(
                table: "Climbs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Validated",
                value: true);

            migrationBuilder.UpdateData(
                table: "CyclistInfos",
                keyColumn: "Id",
                keyValue: 1,
                column: "BirthDate",
                value: new DateTime(2025, 12, 16, 10, 2, 20, 140, DateTimeKind.Local).AddTicks(2129));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Validated",
                table: "Climbs");

            migrationBuilder.UpdateData(
                table: "ActivityInfos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2025, 12, 11, 18, 38, 58, 247, DateTimeKind.Local).AddTicks(8835), new DateTime(2025, 12, 11, 18, 38, 58, 247, DateTimeKind.Local).AddTicks(8833), new DateTime(2025, 12, 11, 18, 8, 58, 247, DateTimeKind.Local).AddTicks(8828) });

            migrationBuilder.UpdateData(
                table: "CyclistInfos",
                keyColumn: "Id",
                keyValue: 1,
                column: "BirthDate",
                value: new DateTime(2025, 12, 11, 18, 38, 58, 247, DateTimeKind.Local).AddTicks(8664));
        }
    }
}
