using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YactAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddFileDetailsToActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Activities",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "Activities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Activities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ContentType", "FileSize", "UploadDate" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "Activities");
        }
    }
}
