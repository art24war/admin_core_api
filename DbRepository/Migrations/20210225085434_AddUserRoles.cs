using Microsoft.EntityFrameworkCore.Migrations;

namespace DbRepository.Migrations
{
    public partial class AddUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Users_UserModelId",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.RenameTable(
                name: "UserRole",
                newName: "Roles");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_UserModelId",
                table: "Roles",
                newName: "IX_Roles_UserModelId");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleGroupId",
                table: "Roles",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RoleGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleGroupId",
                table: "Roles",
                column: "RoleGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_RoleGroups_RoleGroupId",
                table: "Roles",
                column: "RoleGroupId",
                principalTable: "RoleGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_UserModelId",
                table: "Roles",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_RoleGroups_RoleGroupId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_UserModelId",
                table: "Roles");

            migrationBuilder.DropTable(
                name: "RoleGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_RoleGroupId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "RoleGroupId",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "UserRole");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_UserModelId",
                table: "UserRole",
                newName: "IX_UserRole_UserModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Users_UserModelId",
                table: "UserRole",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
