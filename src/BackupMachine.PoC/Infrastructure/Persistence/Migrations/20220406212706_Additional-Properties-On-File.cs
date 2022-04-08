using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackupMachine.PoC.Infrastructure.Persistence.Migrations
{
    public partial class AdditionalPropertiesOnFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "Length",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Files");
        }
    }
}
