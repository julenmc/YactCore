using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActivityClimbTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityClimbs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClimbId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartPointMeters = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityClimbs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityClimbs_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivityClimbs_Climbs_ClimbId",
                        column: x => x.ClimbId,
                        principalTable: "Climbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityClimbs_ActivityId",
                table: "ActivityClimbs",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityClimbs_ClimbId",
                table: "ActivityClimbs",
                column: "ClimbId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityClimbs");
        }
    }
}
