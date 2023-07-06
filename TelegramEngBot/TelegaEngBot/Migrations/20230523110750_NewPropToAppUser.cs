using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegaEngBot.Migrations
{
    /// <inheritdoc />
    public partial class NewPropToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSynced",
                table: "UserList",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivity",
                table: "UserList",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSynced",
                table: "UserList");

            migrationBuilder.DropColumn(
                name: "LastActivity",
                table: "UserList");
        }
    }
}
