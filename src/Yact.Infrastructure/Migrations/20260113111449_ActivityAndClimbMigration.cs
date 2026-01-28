using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActivityAndClimbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CyclistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DistanceMeters = table.Column<double>(type: "float", nullable: false),
                    ElevationMeters = table.Column<double>(type: "float", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Cyclists_CyclistId",
                        column: x => x.CyclistId,
                        principalTable: "Cyclists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Climbs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DistanceMeters = table.Column<double>(type: "float", nullable: false),
                    Slope = table.Column<double>(type: "float", nullable: false),
                    MaxSlope = table.Column<double>(type: "float", nullable: false),
                    NetElevationMeters = table.Column<double>(type: "float", nullable: false),
                    TotalElevationMeters = table.Column<double>(type: "float", nullable: false),
                    LongitudeInit = table.Column<double>(type: "float", nullable: false),
                    LongitudeEnd = table.Column<double>(type: "float", nullable: false),
                    LatitudeInit = table.Column<double>(type: "float", nullable: false),
                    LatitudeEnd = table.Column<double>(type: "float", nullable: false),
                    AltitudeInit = table.Column<double>(type: "float", nullable: false),
                    AltitudeEnd = table.Column<double>(type: "float", nullable: false),
                    StartPointMeters = table.Column<double>(type: "float", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Climbs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CyclistId",
                table: "Activities",
                column: "CyclistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Climbs");
        }
    }
}
