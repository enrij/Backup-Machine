﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackupMachine.PoC.Infrastructure.Persistence.Migrations
{
    public partial class FileStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Files");
        }
    }
}
