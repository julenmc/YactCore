using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActivityClimbToActivityAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityClimbs_Activities_ActivityId",
                table: "ActivityClimbs");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityClimbs_Activities_ActivityId",
                table: "ActivityClimbs",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityClimbs_Activities_ActivityId",
                table: "ActivityClimbs");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityClimbs_Activities_ActivityId",
                table: "ActivityClimbs",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
