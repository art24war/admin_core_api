using Microsoft.EntityFrameworkCore.Migrations;

namespace DbRepository.Migrations
{
    public partial class addUserDealerFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedById",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Users",
                newName: "CreatedByUser");

            migrationBuilder.RenameIndex(
                name: "IX_Users_CreatedById",
                table: "Users",
                newName: "IX_Users_CreatedByUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedByUser",
                table: "Users",
                column: "CreatedByUser",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedByUser",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "CreatedByUser",
                table: "Users",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Users_CreatedByUser",
                table: "Users",
                newName: "IX_Users_CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedById",
                table: "Users",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
