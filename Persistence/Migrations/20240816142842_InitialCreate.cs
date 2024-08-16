using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "'UUID()'", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageLocation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "'UUID()'", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageLocation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "'UUID()'", collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Duration = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    AlbumId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PositionInAlbum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Songs_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AlbumArtistRelations",
                columns: table => new
                {
                    AlbumId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ArtistId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumArtistRelations", x => new { x.AlbumId, x.ArtistId });
                    table.ForeignKey(
                        name: "FK_AlbumArtistRelations_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumArtistRelations_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SongArtistRelations",
                columns: table => new
                {
                    SongId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ArtistId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongArtistRelations", x => new { x.SongId, x.ArtistId });
                    table.ForeignKey(
                        name: "FK_SongArtistRelations_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SongArtistRelations_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "ImageLocation", "Name" },
                values: new object[,]
                {
                    { new Guid("70a957f6-ed66-48de-b72c-47224dc5d70a"), "path/to/image1", "First Album" },
                    { new Guid("8cc948dd-374a-47a2-aba6-52d02151e14b"), "path/to/image2", "Second Album" }
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "ImageLocation", "Name" },
                values: new object[,]
                {
                    { new Guid("4bab74a2-a8d1-483b-bb89-4eaffb704dd5"), "path/to/image2", "Second Artist" },
                    { new Guid("5890e712-b6ce-47e3-a5c3-2ee41bbc3651"), "path/to/image1", "First Artist" }
                });

            migrationBuilder.InsertData(
                table: "AlbumArtistRelations",
                columns: new[] { "AlbumId", "ArtistId" },
                values: new object[,]
                {
                    { new Guid("70a957f6-ed66-48de-b72c-47224dc5d70a"), new Guid("4bab74a2-a8d1-483b-bb89-4eaffb704dd5") },
                    { new Guid("70a957f6-ed66-48de-b72c-47224dc5d70a"), new Guid("5890e712-b6ce-47e3-a5c3-2ee41bbc3651") },
                    { new Guid("8cc948dd-374a-47a2-aba6-52d02151e14b"), new Guid("4bab74a2-a8d1-483b-bb89-4eaffb704dd5") }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "AlbumId", "Duration", "PositionInAlbum", "Title" },
                values: new object[,]
                {
                    { new Guid("39fe0f6f-d521-446c-925c-5887b8c11b97"), new Guid("70a957f6-ed66-48de-b72c-47224dc5d70a"), new TimeSpan(0, 0, 4, 20, 0), -1, "Second Song" },
                    { new Guid("477b3f00-7d39-4472-a79e-b21b48aa1dc0"), new Guid("8cc948dd-374a-47a2-aba6-52d02151e14b"), new TimeSpan(0, 0, 5, 0, 0), -1, "Third Song" },
                    { new Guid("ff72bbf1-75d1-42a4-8f57-83f2392e63e7"), new Guid("70a957f6-ed66-48de-b72c-47224dc5d70a"), new TimeSpan(0, 0, 3, 45, 0), -1, "First Song" }
                });

            migrationBuilder.InsertData(
                table: "SongArtistRelations",
                columns: new[] { "ArtistId", "SongId" },
                values: new object[,]
                {
                    { new Guid("5890e712-b6ce-47e3-a5c3-2ee41bbc3651"), new Guid("39fe0f6f-d521-446c-925c-5887b8c11b97") },
                    { new Guid("4bab74a2-a8d1-483b-bb89-4eaffb704dd5"), new Guid("477b3f00-7d39-4472-a79e-b21b48aa1dc0") },
                    { new Guid("5890e712-b6ce-47e3-a5c3-2ee41bbc3651"), new Guid("ff72bbf1-75d1-42a4-8f57-83f2392e63e7") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumArtistRelation_AlbumId",
                table: "AlbumArtistRelations",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumArtistRelation_ArtistId",
                table: "AlbumArtistRelations",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Album_Name",
                table: "Albums",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Artist_Name",
                table: "Artists",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SongArtistRelation_ArtistId",
                table: "SongArtistRelations",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_SongArtistRelation_SongId",
                table: "SongArtistRelations",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_Song_AlbumId",
                table: "Songs",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Song_Title",
                table: "Songs",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Song_Title_AlbumId",
                table: "Songs",
                columns: new[] { "Title", "AlbumId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumArtistRelations");

            migrationBuilder.DropTable(
                name: "SongArtistRelations");

            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "Songs");

            migrationBuilder.DropTable(
                name: "Albums");
        }
    }
}
