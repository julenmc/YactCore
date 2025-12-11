using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Climbs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LongitudeInit = table.Column<double>(type: "float", nullable: false),
                    LongitudeEnd = table.Column<double>(type: "float", nullable: false),
                    LatitudeInit = table.Column<double>(type: "float", nullable: false),
                    LatitudeEnd = table.Column<double>(type: "float", nullable: false),
                    AltitudeInit = table.Column<double>(type: "float", nullable: false),
                    AltitudeEnd = table.Column<double>(type: "float", nullable: false),
                    DistanceMeters = table.Column<double>(type: "float", nullable: false),
                    Slope = table.Column<double>(type: "float", nullable: false),
                    MaxSlope = table.Column<double>(type: "float", nullable: false),
                    Elevation = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Climbs", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "ActivityInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DistanceMeters = table.Column<double>(type: "float", nullable: false),
                    ElevationMeters = table.Column<double>(type: "float", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CyclistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityInfos_CyclistInfos_CyclistId",
                        column: x => x.CyclistId,
                        principalTable: "CyclistInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CyclistFitnesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Ftp = table.Column<int>(type: "int", nullable: false),
                    Vo2Max = table.Column<float>(type: "real", nullable: false),
                    PowerCurveJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HrZonesRaw = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PowerZonesRaw = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CyclistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CyclistFitnesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CyclistFitnesses_CyclistInfos_CyclistId",
                        column: x => x.CyclistId,
                        principalTable: "CyclistInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityClimbs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartPointMeters = table.Column<double>(type: "float", nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    ClimbId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityClimbs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityClimbs_ActivityInfos_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "ActivityInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityClimbs_Climbs_ClimbId",
                        column: x => x.ClimbId,
                        principalTable: "Climbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Intervals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Distance = table.Column<float>(type: "real", nullable: false),
                    AverageHeartRate = table.Column<float>(type: "real", nullable: false),
                    AveragePower = table.Column<float>(type: "real", nullable: false),
                    AverageCadence = table.Column<float>(type: "real", nullable: false),
                    MaxHeartRate = table.Column<float>(type: "real", nullable: false),
                    MaxPower = table.Column<float>(type: "real", nullable: false),
                    MaxCadence = table.Column<float>(type: "real", nullable: false),
                    ActivityInfoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Intervals_ActivityInfos_ActivityInfoId",
                        column: x => x.ActivityInfoId,
                        principalTable: "ActivityInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Climbs",
                columns: new[] { "Id", "AltitudeEnd", "AltitudeInit", "DistanceMeters", "Elevation", "LatitudeEnd", "LatitudeInit", "LongitudeEnd", "LongitudeInit", "MaxSlope", "Name", "Slope" },
                values: new object[] { 1, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, "Unknown", 0.0 });

            migrationBuilder.InsertData(
                table: "CyclistInfos",
                columns: new[] { "Id", "BirthDate", "LastName", "Name" },
                values: new object[] { 1, new DateTime(2025, 12, 11, 18, 38, 58, 247, DateTimeKind.Local).AddTicks(8664), "Cyclist", "Dummy" });

            migrationBuilder.InsertData(
                table: "ActivityInfos",
                columns: new[] { "Id", "CreateDate", "CyclistId", "Description", "DistanceMeters", "ElevationMeters", "EndDate", "Name", "Path", "StartDate", "Type", "UpdateDate" },
                values: new object[] { 1, new DateTime(2025, 12, 11, 18, 38, 58, 247, DateTimeKind.Local).AddTicks(8835), 1, "This is a dummy activity", 10000.0, 100.0, new DateTime(2025, 12, 11, 18, 38, 58, 247, DateTimeKind.Local).AddTicks(8833), "Dummy Activity", "dummy_activity.fit", new DateTime(2025, 12, 11, 18, 8, 58, 247, DateTimeKind.Local).AddTicks(8828), "Cycling", null });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityClimbs_ActivityId",
                table: "ActivityClimbs",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityClimbs_ClimbId",
                table: "ActivityClimbs",
                column: "ClimbId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityInfos_CyclistId",
                table: "ActivityInfos",
                column: "CyclistId");

            migrationBuilder.CreateIndex(
                name: "IX_CyclistFitnesses_CyclistId",
                table: "CyclistFitnesses",
                column: "CyclistId");

            migrationBuilder.CreateIndex(
                name: "IX_Intervals_ActivityInfoId",
                table: "Intervals",
                column: "ActivityInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityClimbs");

            migrationBuilder.DropTable(
                name: "CyclistFitnesses");

            migrationBuilder.DropTable(
                name: "Intervals");

            migrationBuilder.DropTable(
                name: "Climbs");

            migrationBuilder.DropTable(
                name: "ActivityInfos");

            migrationBuilder.DropTable(
                name: "CyclistInfos");
        }
    }
}
