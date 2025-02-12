using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddArtistCreatedUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("97208fce-2687-4a95-82de-c4346a9ff5e0"), new Guid("408fa13a-5214-43c5-b7d3-c14015f6bdfd") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("d44eee88-7c1e-4d61-8df3-4598a6e9d551"), new Guid("408fa13a-5214-43c5-b7d3-c14015f6bdfd") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("d44eee88-7c1e-4d61-8df3-4598a6e9d551"), new Guid("5f2acbc5-6cbb-4cd2-ac7d-af3e7757cf02") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("5f2acbc5-6cbb-4cd2-ac7d-af3e7757cf02"), new Guid("472130ab-be11-428c-89af-a09960d66736") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("408fa13a-5214-43c5-b7d3-c14015f6bdfd"), new Guid("5eb18db5-3047-45cc-af3a-233f4f963184") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("5f2acbc5-6cbb-4cd2-ac7d-af3e7757cf02"), new Guid("8cfcd0a6-a207-4f59-af48-14f29d5538c4") });

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("408fa13a-5214-43c5-b7d3-c14015f6bdfd"));

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("5f2acbc5-6cbb-4cd2-ac7d-af3e7757cf02"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("472130ab-be11-428c-89af-a09960d66736"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("5eb18db5-3047-45cc-af3a-233f4f963184"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("8cfcd0a6-a207-4f59-af48-14f29d5538c4"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("97208fce-2687-4a95-82de-c4346a9ff5e0"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("d44eee88-7c1e-4d61-8df3-4598a6e9d551"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Artists",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Artists",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Artists");

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "ImageLocation", "Name" },
                values: new object[,]
                {
                    { new Guid("97208fce-2687-4a95-82de-c4346a9ff5e0"), "path/to/image2", "Second Album" },
                    { new Guid("d44eee88-7c1e-4d61-8df3-4598a6e9d551"), "path/to/image1", "First Album" }
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "ImageLocation", "Name" },
                values: new object[,]
                {
                    { new Guid("408fa13a-5214-43c5-b7d3-c14015f6bdfd"), "path/to/image2", "Second Artist" },
                    { new Guid("5f2acbc5-6cbb-4cd2-ac7d-af3e7757cf02"), "path/to/image1", "First Artist" }
                });

            migrationBuilder.InsertData(
                table: "AlbumArtistRelations",
                columns: new[] { "AlbumId", "ArtistId" },
                values: new object[,]
                {
                    { new Guid("97208fce-2687-4a95-82de-c4346a9ff5e0"), new Guid("408fa13a-5214-43c5-b7d3-c14015f6bdfd") },
                    { new Guid("d44eee88-7c1e-4d61-8df3-4598a6e9d551"), new Guid("408fa13a-5214-43c5-b7d3-c14015f6bdfd") },
                    { new Guid("d44eee88-7c1e-4d61-8df3-4598a6e9d551"), new Guid("5f2acbc5-6cbb-4cd2-ac7d-af3e7757cf02") }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "AlbumId", "Duration", "PositionInAlbum", "Title" },
                values: new object[,]
                {
                    { new Guid("472130ab-be11-428c-89af-a09960d66736"), new Guid("d44eee88-7c1e-4d61-8df3-4598a6e9d551"), new TimeSpan(0, 0, 4, 20, 0), -1, "Second Song" },
                    { new Guid("5eb18db5-3047-45cc-af3a-233f4f963184"), new Guid("97208fce-2687-4a95-82de-c4346a9ff5e0"), new TimeSpan(0, 0, 5, 0, 0), -1, "Third Song" },
                    { new Guid("8cfcd0a6-a207-4f59-af48-14f29d5538c4"), new Guid("d44eee88-7c1e-4d61-8df3-4598a6e9d551"), new TimeSpan(0, 0, 3, 45, 0), -1, "First Song" }
                });

            migrationBuilder.InsertData(
                table: "SongArtistRelations",
                columns: new[] { "ArtistId", "SongId" },
                values: new object[,]
                {
                    { new Guid("5f2acbc5-6cbb-4cd2-ac7d-af3e7757cf02"), new Guid("472130ab-be11-428c-89af-a09960d66736") },
                    { new Guid("408fa13a-5214-43c5-b7d3-c14015f6bdfd"), new Guid("5eb18db5-3047-45cc-af3a-233f4f963184") },
                    { new Guid("5f2acbc5-6cbb-4cd2-ac7d-af3e7757cf02"), new Guid("8cfcd0a6-a207-4f59-af48-14f29d5538c4") }
                });
        }
    }
}
