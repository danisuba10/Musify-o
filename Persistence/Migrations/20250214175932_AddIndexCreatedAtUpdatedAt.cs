using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexCreatedAtUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("5e43cf16-0657-42ed-baca-a9c7936bd5a6"), new Guid("b7410454-ed69-4403-a699-9f7e6b9a307d") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("db212c47-9094-4e92-a330-2b92ecfab502"), new Guid("795c69d9-1759-4f4a-ba78-de2d240b7245") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("db212c47-9094-4e92-a330-2b92ecfab502"), new Guid("b7410454-ed69-4403-a699-9f7e6b9a307d") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("795c69d9-1759-4f4a-ba78-de2d240b7245"), new Guid("29f71f05-fc76-428b-9591-7b5cb5bafb5a") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("795c69d9-1759-4f4a-ba78-de2d240b7245"), new Guid("a3cc1977-15f0-48b8-bda5-cf7b1b700821") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("b7410454-ed69-4403-a699-9f7e6b9a307d"), new Guid("e2066fc3-35bd-4b8a-b565-12cfdaef47e8") });

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("795c69d9-1759-4f4a-ba78-de2d240b7245"));

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("b7410454-ed69-4403-a699-9f7e6b9a307d"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("29f71f05-fc76-428b-9591-7b5cb5bafb5a"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("a3cc1977-15f0-48b8-bda5-cf7b1b700821"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("e2066fc3-35bd-4b8a-b565-12cfdaef47e8"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("5e43cf16-0657-42ed-baca-a9c7936bd5a6"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("db212c47-9094-4e92-a330-2b92ecfab502"));

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

            migrationBuilder.CreateIndex(
                name: "IX_Song_CreatedAt",
                table: "Songs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Song_UpdatedAt",
                table: "Songs",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Artist_CreatedAt",
                table: "Artists",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Artist_UpdatedAt",
                table: "Artists",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Album_CreatedAt",
                table: "Albums",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Album_UpdatedAt",
                table: "Albums",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Song_CreatedAt",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Song_UpdatedAt",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Artist_CreatedAt",
                table: "Artists");

            migrationBuilder.DropIndex(
                name: "IX_Artist_UpdatedAt",
                table: "Artists");

            migrationBuilder.DropIndex(
                name: "IX_Album_CreatedAt",
                table: "Albums");

            migrationBuilder.DropIndex(
                name: "IX_Album_UpdatedAt",
                table: "Albums");

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

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "CreatedAt", "ImageLocation", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("5e43cf16-0657-42ed-baca-a9c7936bd5a6"), new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7249), "path/to/image2", "Second Album", new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7249) },
                    { new Guid("db212c47-9094-4e92-a330-2b92ecfab502"), new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7242), "path/to/image1", "First Album", new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7245) }
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "CreatedAt", "ImageLocation", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("795c69d9-1759-4f4a-ba78-de2d240b7245"), new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7469), "path/to/image1", "First Artist", new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7469) },
                    { new Guid("b7410454-ed69-4403-a699-9f7e6b9a307d"), new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7475), "path/to/image2", "Second Artist", new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7475) }
                });

            migrationBuilder.InsertData(
                table: "AlbumArtistRelations",
                columns: new[] { "AlbumId", "ArtistId" },
                values: new object[,]
                {
                    { new Guid("5e43cf16-0657-42ed-baca-a9c7936bd5a6"), new Guid("b7410454-ed69-4403-a699-9f7e6b9a307d") },
                    { new Guid("db212c47-9094-4e92-a330-2b92ecfab502"), new Guid("795c69d9-1759-4f4a-ba78-de2d240b7245") },
                    { new Guid("db212c47-9094-4e92-a330-2b92ecfab502"), new Guid("b7410454-ed69-4403-a699-9f7e6b9a307d") }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "AlbumId", "CreatedAt", "Duration", "PositionInAlbum", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("29f71f05-fc76-428b-9591-7b5cb5bafb5a"), new Guid("db212c47-9094-4e92-a330-2b92ecfab502"), new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7518), new TimeSpan(0, 0, 3, 45, 0), -1, "First Song", new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7519) },
                    { new Guid("a3cc1977-15f0-48b8-bda5-cf7b1b700821"), new Guid("db212c47-9094-4e92-a330-2b92ecfab502"), new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7526), new TimeSpan(0, 0, 4, 20, 0), -1, "Second Song", new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7527) },
                    { new Guid("e2066fc3-35bd-4b8a-b565-12cfdaef47e8"), new Guid("5e43cf16-0657-42ed-baca-a9c7936bd5a6"), new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7529), new TimeSpan(0, 0, 5, 0, 0), -1, "Third Song", new DateTime(2025, 2, 14, 13, 57, 31, 822, DateTimeKind.Utc).AddTicks(7530) }
                });

            migrationBuilder.InsertData(
                table: "SongArtistRelations",
                columns: new[] { "ArtistId", "SongId" },
                values: new object[,]
                {
                    { new Guid("795c69d9-1759-4f4a-ba78-de2d240b7245"), new Guid("29f71f05-fc76-428b-9591-7b5cb5bafb5a") },
                    { new Guid("795c69d9-1759-4f4a-ba78-de2d240b7245"), new Guid("a3cc1977-15f0-48b8-bda5-cf7b1b700821") },
                    { new Guid("b7410454-ed69-4403-a699-9f7e6b9a307d"), new Guid("e2066fc3-35bd-4b8a-b565-12cfdaef47e8") }
                });
        }
    }
}
