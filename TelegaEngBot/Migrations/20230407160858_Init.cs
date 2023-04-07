using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegaEngBot.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommonVocabulary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EngWord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RusWord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlLink = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonVocabulary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalArticlesWeight = table.Column<int>(type: "int", nullable: false),
                    UserSettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserList_UserSettings_UserSettingsId",
                        column: x => x.UserSettingsId,
                        principalTable: "UserSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVocabularyItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVocabularyItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVocabularyItem_CommonVocabulary_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "CommonVocabulary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserVocabularyItem_UserList_UserId",
                        column: x => x.UserId,
                        principalTable: "UserList",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserList_UserSettingsId",
                table: "UserList",
                column: "UserSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVocabularyItem_ArticleId",
                table: "UserVocabularyItem",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVocabularyItem_UserId",
                table: "UserVocabularyItem",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserVocabularyItem");

            migrationBuilder.DropTable(
                name: "CommonVocabulary");

            migrationBuilder.DropTable(
                name: "UserList");

            migrationBuilder.DropTable(
                name: "UserSettings");
        }
    }
}
