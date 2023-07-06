using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegaEngBot.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserList_UserSettings_UserSettingsId",
                table: "UserList");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVocabularyItem_CommonVocabulary_ArticleId",
                table: "UserVocabularyItem");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticleId",
                table: "UserVocabularyItem",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserSettingsId",
                table: "UserList",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "LastArticleId",
                table: "UserList",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RusWord",
                table: "CommonVocabulary",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EngWord",
                table: "CommonVocabulary",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserList_LastArticleId",
                table: "UserList",
                column: "LastArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserList_CommonVocabulary_LastArticleId",
                table: "UserList",
                column: "LastArticleId",
                principalTable: "CommonVocabulary",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserList_UserSettings_UserSettingsId",
                table: "UserList",
                column: "UserSettingsId",
                principalTable: "UserSettings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserVocabularyItem_CommonVocabulary_ArticleId",
                table: "UserVocabularyItem",
                column: "ArticleId",
                principalTable: "CommonVocabulary",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserList_CommonVocabulary_LastArticleId",
                table: "UserList");

            migrationBuilder.DropForeignKey(
                name: "FK_UserList_UserSettings_UserSettingsId",
                table: "UserList");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVocabularyItem_CommonVocabulary_ArticleId",
                table: "UserVocabularyItem");

            migrationBuilder.DropIndex(
                name: "IX_UserList_LastArticleId",
                table: "UserList");

            migrationBuilder.DropColumn(
                name: "LastArticleId",
                table: "UserList");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticleId",
                table: "UserVocabularyItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserSettingsId",
                table: "UserList",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RusWord",
                table: "CommonVocabulary",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EngWord",
                table: "CommonVocabulary",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserList_UserSettings_UserSettingsId",
                table: "UserList",
                column: "UserSettingsId",
                principalTable: "UserSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVocabularyItem_CommonVocabulary_ArticleId",
                table: "UserVocabularyItem",
                column: "ArticleId",
                principalTable: "CommonVocabulary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
