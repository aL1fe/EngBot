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
                name: "Dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EngWord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RusWord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlLink = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDictionary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDictionary_Dictionary_WordId",
                        column: x => x.WordId,
                        principalTable: "Dictionary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDictionary_WordId",
                table: "UserDictionary",
                column: "WordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDictionary");

            migrationBuilder.DropTable(
                name: "UserList");

            migrationBuilder.DropTable(
                name: "Dictionary");
        }
    }
}
