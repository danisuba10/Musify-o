using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserLibraryItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLibraryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ItemId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ItemType = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SavedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLibraryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLibraryItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserLibraryItem_SavedAt",
                table: "UserLibraryItems",
                column: "SavedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserLibraryItem_UserId_ItemType_ItemId",
                table: "UserLibraryItems",
                columns: new[] { "UserId", "ItemType", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLibraryItem_UserId_SavedAt",
                table: "UserLibraryItems",
                columns: new[] { "UserId", "SavedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLibraryItems");
        }
    }
}
