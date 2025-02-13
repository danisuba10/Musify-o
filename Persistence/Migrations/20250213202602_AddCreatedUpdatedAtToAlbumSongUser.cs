using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedUpdatedAtToAlbumSongUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("2f227d3d-473f-4bfb-8f77-2400ddeba03b"), new Guid("866f690b-f411-4ebc-8ff6-f64d8bbbbe6c") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("bcfdc9e8-fdd7-4f00-b398-11e9b757ddce"), new Guid("144b3ba5-03df-4ea3-abde-9fff51549409") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("bcfdc9e8-fdd7-4f00-b398-11e9b757ddce"), new Guid("866f690b-f411-4ebc-8ff6-f64d8bbbbe6c") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("866f690b-f411-4ebc-8ff6-f64d8bbbbe6c"), new Guid("4170612e-38f0-46a9-9d6c-ed99e88bceb5") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("144b3ba5-03df-4ea3-abde-9fff51549409"), new Guid("7d489738-ab06-41dd-8b8f-796496a62a91") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("144b3ba5-03df-4ea3-abde-9fff51549409"), new Guid("e5f95f6c-f847-4306-9cd3-5ce5abe54a3f") });

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("144b3ba5-03df-4ea3-abde-9fff51549409"));

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("866f690b-f411-4ebc-8ff6-f64d8bbbbe6c"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("4170612e-38f0-46a9-9d6c-ed99e88bceb5"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("7d489738-ab06-41dd-8b8f-796496a62a91"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("e5f95f6c-f847-4306-9cd3-5ce5abe54a3f"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("2f227d3d-473f-4bfb-8f77-2400ddeba03b"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("bcfdc9e8-fdd7-4f00-b398-11e9b757ddce"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Songs",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Songs",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Albums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Albums",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Albums");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Albums");

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "ImageLocation", "Name" },
                values: new object[,]
                {
                    { new Guid("2f227d3d-473f-4bfb-8f77-2400ddeba03b"), "path/to/image2", "Second Album" },
                    { new Guid("bcfdc9e8-fdd7-4f00-b398-11e9b757ddce"), "path/to/image1", "First Album" }
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "CreatedAt", "ImageLocation", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("144b3ba5-03df-4ea3-abde-9fff51549409"), new DateTime(2025, 2, 12, 18, 15, 36, 285, DateTimeKind.Utc).AddTicks(2461), "path/to/image1", "First Artist", new DateTime(2025, 2, 12, 18, 15, 36, 285, DateTimeKind.Utc).AddTicks(2463) },
                    { new Guid("866f690b-f411-4ebc-8ff6-f64d8bbbbe6c"), new DateTime(2025, 2, 12, 18, 15, 36, 285, DateTimeKind.Utc).AddTicks(2471), "path/to/image2", "Second Artist", new DateTime(2025, 2, 12, 18, 15, 36, 285, DateTimeKind.Utc).AddTicks(2471) }
                });

            migrationBuilder.InsertData(
                table: "AlbumArtistRelations",
                columns: new[] { "AlbumId", "ArtistId" },
                values: new object[,]
                {
                    { new Guid("2f227d3d-473f-4bfb-8f77-2400ddeba03b"), new Guid("866f690b-f411-4ebc-8ff6-f64d8bbbbe6c") },
                    { new Guid("bcfdc9e8-fdd7-4f00-b398-11e9b757ddce"), new Guid("144b3ba5-03df-4ea3-abde-9fff51549409") },
                    { new Guid("bcfdc9e8-fdd7-4f00-b398-11e9b757ddce"), new Guid("866f690b-f411-4ebc-8ff6-f64d8bbbbe6c") }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "AlbumId", "Duration", "PositionInAlbum", "Title" },
                values: new object[,]
                {
                    { new Guid("4170612e-38f0-46a9-9d6c-ed99e88bceb5"), new Guid("2f227d3d-473f-4bfb-8f77-2400ddeba03b"), new TimeSpan(0, 0, 5, 0, 0), -1, "Third Song" },
                    { new Guid("7d489738-ab06-41dd-8b8f-796496a62a91"), new Guid("bcfdc9e8-fdd7-4f00-b398-11e9b757ddce"), new TimeSpan(0, 0, 3, 45, 0), -1, "First Song" },
                    { new Guid("e5f95f6c-f847-4306-9cd3-5ce5abe54a3f"), new Guid("bcfdc9e8-fdd7-4f00-b398-11e9b757ddce"), new TimeSpan(0, 0, 4, 20, 0), -1, "Second Song" }
                });

            migrationBuilder.InsertData(
                table: "SongArtistRelations",
                columns: new[] { "ArtistId", "SongId" },
                values: new object[,]
                {
                    { new Guid("866f690b-f411-4ebc-8ff6-f64d8bbbbe6c"), new Guid("4170612e-38f0-46a9-9d6c-ed99e88bceb5") },
                    { new Guid("144b3ba5-03df-4ea3-abde-9fff51549409"), new Guid("7d489738-ab06-41dd-8b8f-796496a62a91") },
                    { new Guid("144b3ba5-03df-4ea3-abde-9fff51549409"), new Guid("e5f95f6c-f847-4306-9cd3-5ce5abe54a3f") }
                });
        }
    }
}
