using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegaEngBot.Migrations
{
    /// <inheritdoc />
    public partial class DifficultyLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DifficultyLevel",
                table: "UserSettings",
                type: "int",
                nullable: true,
                defaultValue: null);

            migrationBuilder.AddColumn<int>(
                name: "DifficultyLevel",
                table: "CommonVocabulary",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "CommonVocabulary");
        }
    }
}
