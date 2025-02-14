using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserChangeUserNameToEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("3d29240d-dbef-4b2c-8b3e-63c72ca6ecf4"), new Guid("807bddea-7d0e-4d08-a7f2-b65dc5d12309") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("3d29240d-dbef-4b2c-8b3e-63c72ca6ecf4"), new Guid("c89dad75-9826-43e3-9d7d-3b8c0823281b") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("bca35c96-417d-4a05-b71c-9321897e3e94"), new Guid("807bddea-7d0e-4d08-a7f2-b65dc5d12309") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("c89dad75-9826-43e3-9d7d-3b8c0823281b"), new Guid("1477bed5-ec5f-49d8-af90-3ba79752ecb2") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("c89dad75-9826-43e3-9d7d-3b8c0823281b"), new Guid("77cd3df6-36bd-4d75-a892-ff6bb3c12c52") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("807bddea-7d0e-4d08-a7f2-b65dc5d12309"), new Guid("911de022-884e-44a2-8b43-b3b951cddee7") });

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("807bddea-7d0e-4d08-a7f2-b65dc5d12309"));

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("c89dad75-9826-43e3-9d7d-3b8c0823281b"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("1477bed5-ec5f-49d8-af90-3ba79752ecb2"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("77cd3df6-36bd-4d75-a892-ff6bb3c12c52"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("911de022-884e-44a2-8b43-b3b951cddee7"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("3d29240d-dbef-4b2c-8b3e-63c72ca6ecf4"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("bca35c96-417d-4a05-b71c-9321897e3e94"));

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "Email");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "UserName");

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "CreatedAt", "ImageLocation", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("3d29240d-dbef-4b2c-8b3e-63c72ca6ecf4"), new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(3876), "path/to/image1", "First Album", new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(3878) },
                    { new Guid("bca35c96-417d-4a05-b71c-9321897e3e94"), new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(3882), "path/to/image2", "Second Album", new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(3882) }
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "CreatedAt", "ImageLocation", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("807bddea-7d0e-4d08-a7f2-b65dc5d12309"), new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(4030), "path/to/image2", "Second Artist", new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(4030) },
                    { new Guid("c89dad75-9826-43e3-9d7d-3b8c0823281b"), new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(4018), "path/to/image1", "First Artist", new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(4018) }
                });

            migrationBuilder.InsertData(
                table: "AlbumArtistRelations",
                columns: new[] { "AlbumId", "ArtistId" },
                values: new object[,]
                {
                    { new Guid("3d29240d-dbef-4b2c-8b3e-63c72ca6ecf4"), new Guid("807bddea-7d0e-4d08-a7f2-b65dc5d12309") },
                    { new Guid("3d29240d-dbef-4b2c-8b3e-63c72ca6ecf4"), new Guid("c89dad75-9826-43e3-9d7d-3b8c0823281b") },
                    { new Guid("bca35c96-417d-4a05-b71c-9321897e3e94"), new Guid("807bddea-7d0e-4d08-a7f2-b65dc5d12309") }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "AlbumId", "CreatedAt", "Duration", "PositionInAlbum", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1477bed5-ec5f-49d8-af90-3ba79752ecb2"), new Guid("3d29240d-dbef-4b2c-8b3e-63c72ca6ecf4"), new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(4070), new TimeSpan(0, 0, 4, 20, 0), -1, "Second Song", new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(4071) },
                    { new Guid("77cd3df6-36bd-4d75-a892-ff6bb3c12c52"), new Guid("3d29240d-dbef-4b2c-8b3e-63c72ca6ecf4"), new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(4061), new TimeSpan(0, 0, 3, 45, 0), -1, "First Song", new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(4062) },
                    { new Guid("911de022-884e-44a2-8b43-b3b951cddee7"), new Guid("bca35c96-417d-4a05-b71c-9321897e3e94"), new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(4073), new TimeSpan(0, 0, 5, 0, 0), -1, "Third Song", new DateTime(2025, 2, 13, 20, 26, 1, 492, DateTimeKind.Utc).AddTicks(4073) }
                });

            migrationBuilder.InsertData(
                table: "SongArtistRelations",
                columns: new[] { "ArtistId", "SongId" },
                values: new object[,]
                {
                    { new Guid("c89dad75-9826-43e3-9d7d-3b8c0823281b"), new Guid("1477bed5-ec5f-49d8-af90-3ba79752ecb2") },
                    { new Guid("c89dad75-9826-43e3-9d7d-3b8c0823281b"), new Guid("77cd3df6-36bd-4d75-a892-ff6bb3c12c52") },
                    { new Guid("807bddea-7d0e-4d08-a7f2-b65dc5d12309"), new Guid("911de022-884e-44a2-8b43-b3b951cddee7") }
                });
        }
    }
}
