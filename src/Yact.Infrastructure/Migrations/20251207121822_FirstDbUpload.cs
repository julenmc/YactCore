using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstDbUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CyclistId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DistanceMeters = table.Column<double>(type: "float", nullable: false),
                    ElevationMeters = table.Column<double>(type: "float", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CyclistFitnesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CyclistId = table.Column<int>(type: "int", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Ftp = table.Column<int>(type: "int", nullable: false),
                    Vo2Max = table.Column<float>(type: "real", nullable: false),
                    PowerCurveJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HrZonesRaw = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PowerZonesRaw = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CyclistFitnesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CyclistInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CyclistInfos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Activities",
                columns: new[] { "Id", "CreateDate", "CyclistId", "Description", "DistanceMeters", "ElevationMeters", "EndDate", "Name", "Path", "StartDate", "Type", "UpdateDate" },
                values: new object[] { 1, new DateTime(2025, 12, 7, 13, 18, 22, 92, DateTimeKind.Local).AddTicks(7982), 1, "This is a dummy activity", 10000.0, 100.0, new DateTime(2025, 12, 7, 13, 18, 22, 92, DateTimeKind.Local).AddTicks(7980), "Dummy Activity", "dummy_activity.fit", new DateTime(2025, 12, 7, 12, 48, 22, 92, DateTimeKind.Local).AddTicks(7975), "Cycling", null });

            migrationBuilder.InsertData(
                table: "CyclistInfos",
                columns: new[] { "Id", "BirthDate", "LastName", "Name" },
                values: new object[] { 1, new DateTime(2025, 12, 7, 13, 18, 22, 92, DateTimeKind.Local).AddTicks(7775), "Cyclist", "Dummy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "CyclistFitnesses");

            migrationBuilder.DropTable(
                name: "CyclistInfos");
        }
    }
}
