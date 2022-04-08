using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackupMachine.PoC.Infrastructure.Persistence.Migrations
{
    public partial class PreviousBackupRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PreviousBackupId",
                table: "Backups",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Backups_PreviousBackupId",
                table: "Backups",
                column: "PreviousBackupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Backups_Backups_PreviousBackupId",
                table: "Backups",
                column: "PreviousBackupId",
                principalTable: "Backups",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Backups_Backups_PreviousBackupId",
                table: "Backups");

            migrationBuilder.DropIndex(
                name: "IX_Backups_PreviousBackupId",
                table: "Backups");

            migrationBuilder.DropColumn(
                name: "PreviousBackupId",
                table: "Backups");
        }
    }
}
