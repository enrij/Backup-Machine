using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackupMachine.PoC.Infrastructure.Persistence.Migrations
{
    public partial class RelativePathOnFolder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destination",
                table: "Folders");

            migrationBuilder.RenameColumn(
                name: "Source",
                table: "Folders",
                newName: "RelativePath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RelativePath",
                table: "Folders",
                newName: "Source");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "Folders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
