using Microsoft.EntityFrameworkCore.Migrations;

namespace DbRepository.Migrations
{
    public partial class UserRolesRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleCode",
                table: "Roles");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Roles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserRolesList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleGroupId = table.Column<int>(type: "int", nullable: true),
                    Coment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRolesList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRolesList_RoleGroups_RoleGroupId",
                        column: x => x.RoleGroupId,
                        principalTable: "RoleGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleId",
                table: "Roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRolesList_RoleGroupId",
                table: "UserRolesList",
                column: "RoleGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_UserRolesList_RoleId",
                table: "Roles",
                column: "RoleId",
                principalTable: "UserRolesList",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_UserRolesList_RoleId",
                table: "Roles");

            migrationBuilder.DropTable(
                name: "UserRolesList");

            migrationBuilder.DropIndex(
                name: "IX_Roles_RoleId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Roles");

            migrationBuilder.AddColumn<string>(
                name: "RoleCode",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
