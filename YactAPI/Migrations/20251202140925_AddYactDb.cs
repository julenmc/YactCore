using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YactAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddYactDb : Migration
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Climbs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LongitudeInit = table.Column<double>(type: "float", nullable: false),
                    LongitudeEnd = table.Column<double>(type: "float", nullable: false),
                    LatitudeInit = table.Column<double>(type: "float", nullable: false),
                    LatitudeEnd = table.Column<double>(type: "float", nullable: false),
                    AltitudeInit = table.Column<double>(type: "float", nullable: false),
                    AltitudeEnd = table.Column<double>(type: "float", nullable: false),
                    InitRouteDistance = table.Column<double>(type: "float", nullable: false),
                    EndRouteDistance = table.Column<double>(type: "float", nullable: false),
                    Distance = table.Column<double>(type: "float", nullable: false),
                    AverageSlope = table.Column<double>(type: "float", nullable: false),
                    MaxSlope = table.Column<double>(type: "float", nullable: false),
                    HeightDiff = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Climbs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Activities",
                columns: new[] { "Id", "Name", "Path" },
                values: new object[] { 1, "Dummy Activity", "dummy_activity.fit" });

            migrationBuilder.InsertData(
                table: "Climbs",
                columns: new[] { "Id", "AltitudeEnd", "AltitudeInit", "AverageSlope", "Distance", "EndRouteDistance", "HeightDiff", "InitRouteDistance", "LatitudeEnd", "LatitudeInit", "LongitudeEnd", "LongitudeInit", "MaxSlope", "Name", "Path" },
                values: new object[] { 1, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, "Dummy Climb", "" });
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
