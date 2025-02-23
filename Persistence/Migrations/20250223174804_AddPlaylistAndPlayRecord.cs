using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaylistAndPlayRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "'UUID()'", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playlists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlayRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlayedItemType = table.Column<int>(type: "int", nullable: false),
                    PlayedItemId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PlaylistSongRelations",
                columns: table => new
                {
                    PlaylistId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SongId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PositionInPlaylist = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistSongRelations", x => new { x.PlaylistId, x.SongId });
                    table.ForeignKey(
                        name: "FK_PlaylistSongRelations_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistSongRelations_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_UserId",
                table: "Playlists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistSongRelation_PlaylistId",
                table: "PlaylistSongRelations",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistSongRelation_SongId",
                table: "PlaylistSongRelations",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayRecords_PlayedItemType_PlayedItemId",
                table: "PlayRecords",
                columns: new[] { "PlayedItemType", "PlayedItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayRecords_Timestamp",
                table: "PlayRecords",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_PlayRecords_UserId",
                table: "PlayRecords",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaylistSongRelations");

            migrationBuilder.DropTable(
                name: "PlayRecords");

            migrationBuilder.DropTable(
                name: "Playlists");
        }
    }
}
