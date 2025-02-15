using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateImageAccent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("6c5b4262-d2e3-4924-a2bc-6e6643c3c82b"), new Guid("2ea18738-07ea-4d07-b3da-d4ab5f228c8d") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("c1c66a87-76f9-4894-a2e3-e0e03d7da038"), new Guid("2ea18738-07ea-4d07-b3da-d4ab5f228c8d") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("c1c66a87-76f9-4894-a2e3-e0e03d7da038"), new Guid("ad70866f-8b2e-4a44-9e9e-fc11387ee4cc") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("2ea18738-07ea-4d07-b3da-d4ab5f228c8d"), new Guid("6dbb4cab-45c3-4418-a8d8-ea234baddaed") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("ad70866f-8b2e-4a44-9e9e-fc11387ee4cc"), new Guid("bf3d8fd2-e522-4a58-a73d-2ba26c323e82") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("ad70866f-8b2e-4a44-9e9e-fc11387ee4cc"), new Guid("cc551ebc-5cb0-4792-bbdb-e431f221921f") });

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("2ea18738-07ea-4d07-b3da-d4ab5f228c8d"));

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("ad70866f-8b2e-4a44-9e9e-fc11387ee4cc"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("6dbb4cab-45c3-4418-a8d8-ea234baddaed"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("bf3d8fd2-e522-4a58-a73d-2ba26c323e82"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("cc551ebc-5cb0-4792-bbdb-e431f221921f"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("6c5b4262-d2e3-4924-a2bc-6e6643c3c82b"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("c1c66a87-76f9-4894-a2e3-e0e03d7da038"));

            migrationBuilder.CreateTable(
                name: "ImageAccents",
                columns: table => new
                {
                    ImagePath = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LowAccent = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MiddleAccent = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HighAccent = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageAccents", x => x.ImagePath);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageAccents");

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "CreatedAt", "ImageLocation", "Name", "ReleaseYear", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("6c5b4262-d2e3-4924-a2bc-6e6643c3c82b"), new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(5927), "path/to/image2", "Second Album", 0, new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(5927) },
                    { new Guid("c1c66a87-76f9-4894-a2e3-e0e03d7da038"), new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(5922), "path/to/image1", "First Album", 0, new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(5924) }
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "CreatedAt", "ImageLocation", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2ea18738-07ea-4d07-b3da-d4ab5f228c8d"), new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(6121), "path/to/image2", "Second Artist", new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(6121) },
                    { new Guid("ad70866f-8b2e-4a44-9e9e-fc11387ee4cc"), new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(6115), "path/to/image1", "First Artist", new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(6116) }
                });

            migrationBuilder.InsertData(
                table: "AlbumArtistRelations",
                columns: new[] { "AlbumId", "ArtistId" },
                values: new object[,]
                {
                    { new Guid("6c5b4262-d2e3-4924-a2bc-6e6643c3c82b"), new Guid("2ea18738-07ea-4d07-b3da-d4ab5f228c8d") },
                    { new Guid("c1c66a87-76f9-4894-a2e3-e0e03d7da038"), new Guid("2ea18738-07ea-4d07-b3da-d4ab5f228c8d") },
                    { new Guid("c1c66a87-76f9-4894-a2e3-e0e03d7da038"), new Guid("ad70866f-8b2e-4a44-9e9e-fc11387ee4cc") }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "AlbumId", "CreatedAt", "Duration", "PositionInAlbum", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("6dbb4cab-45c3-4418-a8d8-ea234baddaed"), new Guid("6c5b4262-d2e3-4924-a2bc-6e6643c3c82b"), new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(6167), new TimeSpan(0, 0, 5, 0, 0), -1, "Third Song", new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(6167) },
                    { new Guid("bf3d8fd2-e522-4a58-a73d-2ba26c323e82"), new Guid("c1c66a87-76f9-4894-a2e3-e0e03d7da038"), new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(6165), new TimeSpan(0, 0, 4, 20, 0), -1, "Second Song", new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(6165) },
                    { new Guid("cc551ebc-5cb0-4792-bbdb-e431f221921f"), new Guid("c1c66a87-76f9-4894-a2e3-e0e03d7da038"), new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(6156), new TimeSpan(0, 0, 3, 45, 0), -1, "First Song", new DateTime(2025, 2, 15, 18, 29, 7, 545, DateTimeKind.Utc).AddTicks(6157) }
                });

            migrationBuilder.InsertData(
                table: "SongArtistRelations",
                columns: new[] { "ArtistId", "SongId" },
                values: new object[,]
                {
                    { new Guid("2ea18738-07ea-4d07-b3da-d4ab5f228c8d"), new Guid("6dbb4cab-45c3-4418-a8d8-ea234baddaed") },
                    { new Guid("ad70866f-8b2e-4a44-9e9e-fc11387ee4cc"), new Guid("bf3d8fd2-e522-4a58-a73d-2ba26c323e82") },
                    { new Guid("ad70866f-8b2e-4a44-9e9e-fc11387ee4cc"), new Guid("cc551ebc-5cb0-4792-bbdb-e431f221921f") }
                });
        }
    }
}
