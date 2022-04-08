using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackupMachine.Infrastructure.Persistence.Migrations
{
    public partial class Jobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: false),
                    Destination = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "Destination", "Name", "Source" },
                values: new object[] { new Guid("e005279a-2b23-4a3c-b798-27cb443daf9e"), "C:\\Temp\\Backups\\SmallSource", "Test", "C:\\Temp\\Sources\\SmallSource" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
