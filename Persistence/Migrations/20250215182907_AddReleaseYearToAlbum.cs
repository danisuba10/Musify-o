using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReleaseYearToAlbum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("51c2241a-bab5-4906-b3ae-c31dfe7e4d64"), new Guid("4c77ab33-d2a9-45f7-91ab-9308502a4b4e") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("51c2241a-bab5-4906-b3ae-c31dfe7e4d64"), new Guid("f363512f-5b08-4da5-9a63-565a9752780a") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("75317369-1258-419c-ade8-692b5099cc2d"), new Guid("f363512f-5b08-4da5-9a63-565a9752780a") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("f363512f-5b08-4da5-9a63-565a9752780a"), new Guid("2499f354-eea1-45ff-9f1c-d6fc6cb717c2") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("4c77ab33-d2a9-45f7-91ab-9308502a4b4e"), new Guid("2ef08cc2-33b0-4c3b-925b-68b2483a215c") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("4c77ab33-d2a9-45f7-91ab-9308502a4b4e"), new Guid("dc2a5a61-c8a9-48ab-85d2-8f6e05576d83") });

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("4c77ab33-d2a9-45f7-91ab-9308502a4b4e"));

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("f363512f-5b08-4da5-9a63-565a9752780a"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("2499f354-eea1-45ff-9f1c-d6fc6cb717c2"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("2ef08cc2-33b0-4c3b-925b-68b2483a215c"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("dc2a5a61-c8a9-48ab-85d2-8f6e05576d83"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("51c2241a-bab5-4906-b3ae-c31dfe7e4d64"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("75317369-1258-419c-ade8-692b5099cc2d"));

            migrationBuilder.AddColumn<int>(
                name: "ReleaseYear",
                table: "Albums",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "ReleaseYear",
                table: "Albums");

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "CreatedAt", "ImageLocation", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("51c2241a-bab5-4906-b3ae-c31dfe7e4d64"), new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9355), "path/to/image1", "First Album", new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9358) },
                    { new Guid("75317369-1258-419c-ade8-692b5099cc2d"), new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9361), "path/to/image2", "Second Album", new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9361) }
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "CreatedAt", "ImageLocation", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("4c77ab33-d2a9-45f7-91ab-9308502a4b4e"), new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9516), "path/to/image1", "First Artist", new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9517) },
                    { new Guid("f363512f-5b08-4da5-9a63-565a9752780a"), new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9527), "path/to/image2", "Second Artist", new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9528) }
                });

            migrationBuilder.InsertData(
                table: "AlbumArtistRelations",
                columns: new[] { "AlbumId", "ArtistId" },
                values: new object[,]
                {
                    { new Guid("51c2241a-bab5-4906-b3ae-c31dfe7e4d64"), new Guid("4c77ab33-d2a9-45f7-91ab-9308502a4b4e") },
                    { new Guid("51c2241a-bab5-4906-b3ae-c31dfe7e4d64"), new Guid("f363512f-5b08-4da5-9a63-565a9752780a") },
                    { new Guid("75317369-1258-419c-ade8-692b5099cc2d"), new Guid("f363512f-5b08-4da5-9a63-565a9752780a") }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "AlbumId", "CreatedAt", "Duration", "PositionInAlbum", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2499f354-eea1-45ff-9f1c-d6fc6cb717c2"), new Guid("75317369-1258-419c-ade8-692b5099cc2d"), new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9648), new TimeSpan(0, 0, 5, 0, 0), -1, "Third Song", new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9649) },
                    { new Guid("2ef08cc2-33b0-4c3b-925b-68b2483a215c"), new Guid("51c2241a-bab5-4906-b3ae-c31dfe7e4d64"), new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9637), new TimeSpan(0, 0, 3, 45, 0), -1, "First Song", new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9637) },
                    { new Guid("dc2a5a61-c8a9-48ab-85d2-8f6e05576d83"), new Guid("51c2241a-bab5-4906-b3ae-c31dfe7e4d64"), new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9645), new TimeSpan(0, 0, 4, 20, 0), -1, "Second Song", new DateTime(2025, 2, 14, 17, 59, 31, 959, DateTimeKind.Utc).AddTicks(9646) }
                });

            migrationBuilder.InsertData(
                table: "SongArtistRelations",
                columns: new[] { "ArtistId", "SongId" },
                values: new object[,]
                {
                    { new Guid("f363512f-5b08-4da5-9a63-565a9752780a"), new Guid("2499f354-eea1-45ff-9f1c-d6fc6cb717c2") },
                    { new Guid("4c77ab33-d2a9-45f7-91ab-9308502a4b4e"), new Guid("2ef08cc2-33b0-4c3b-925b-68b2483a215c") },
                    { new Guid("4c77ab33-d2a9-45f7-91ab-9308502a4b4e"), new Guid("dc2a5a61-c8a9-48ab-85d2-8f6e05576d83") }
                });
        }
    }
}
