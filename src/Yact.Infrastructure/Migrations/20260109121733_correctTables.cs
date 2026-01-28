using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yact.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class correctTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cyclists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cyclists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CyclistFitnesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CyclistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    FTP = table.Column<int>(type: "int", nullable: false),
                    VO2Max = table.Column<float>(type: "real", nullable: false),
                    PowerCurve = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HrZones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PowerZones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CyclistFitnesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CyclistFitnesses_Cyclists_CyclistId",
                        column: x => x.CyclistId,
                        principalTable: "Cyclists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CyclistFitnesses_CyclistId_UpdateDate",
                table: "CyclistFitnesses",
                columns: new[] { "CyclistId", "UpdateDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CyclistFitnesses");

            migrationBuilder.DropTable(
                name: "Cyclists");
        }
    }
}
