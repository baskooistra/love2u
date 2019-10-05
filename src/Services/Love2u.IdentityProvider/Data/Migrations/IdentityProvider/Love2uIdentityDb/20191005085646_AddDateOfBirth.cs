using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Love2u.IdentityProvider.Data.Migrations.IdentityProvider.Love2uIdentityDb
{
    public partial class AddDateOfBirth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateBirth",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateBirth",
                table: "AspNetUsers");
        }
    }
}
