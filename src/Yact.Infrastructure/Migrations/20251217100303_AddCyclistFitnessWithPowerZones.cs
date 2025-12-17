using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCyclistFitnessWithPowerZones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ActivityInfos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2025, 12, 17, 11, 3, 2, 490, DateTimeKind.Local).AddTicks(9097), new DateTime(2025, 12, 17, 11, 3, 2, 490, DateTimeKind.Local).AddTicks(9094), new DateTime(2025, 12, 17, 10, 33, 2, 490, DateTimeKind.Local).AddTicks(9089) });

            migrationBuilder.InsertData(
                table: "CyclistFitnesses",
                columns: new[] { "Id", "CyclistId", "Ftp", "Height", "HrZonesRaw", "PowerCurveJson", "PowerZonesRaw", "UpdateDate", "Vo2Max", "Weight" },
                values: new object[] { 1, 1, 250, 180, null, null, "[\r\n                {\"lowLimit\": 0, \"highLimit\":150},\r\n                {\"lowLimit\": 151, \"highLimit\": 200},\r\n                {\"lowLimit\": 201, \"highLimit\": 250},\r\n                {\"lowLimit\": 251, \"highLimit\": 300},\r\n                {\"lowLimit\": 301, \"highLimit\": 350},\r\n                {\"lowLimit\": 351, \"highLimit\": 400},\r\n                {\"lowLimit\": 401, \"highLimit\": 500}\r\n            ]", new DateTime(2025, 12, 17, 11, 3, 2, 490, DateTimeKind.Local).AddTicks(8979), 50f, 70f });

            migrationBuilder.UpdateData(
                table: "CyclistInfos",
                keyColumn: "Id",
                keyValue: 1,
                column: "BirthDate",
                value: new DateTime(2025, 12, 17, 11, 3, 2, 490, DateTimeKind.Local).AddTicks(8782));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CyclistFitnesses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "ActivityInfos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "EndDate", "StartDate" },
                values: new object[] { new DateTime(2025, 12, 16, 10, 2, 20, 140, DateTimeKind.Local).AddTicks(2310), new DateTime(2025, 12, 16, 10, 2, 20, 140, DateTimeKind.Local).AddTicks(2307), new DateTime(2025, 12, 16, 9, 32, 20, 140, DateTimeKind.Local).AddTicks(2303) });

            migrationBuilder.UpdateData(
                table: "CyclistInfos",
                keyColumn: "Id",
                keyValue: 1,
                column: "BirthDate",
                value: new DateTime(2025, 12, 16, 10, 2, 20, 140, DateTimeKind.Local).AddTicks(2129));
        }
    }
}
