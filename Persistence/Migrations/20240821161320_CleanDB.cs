using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CleanDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("3646c63b-410a-447c-9019-154d07600fee"), new Guid("47a6853b-a9c7-48a1-8292-5566e754459a") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("e1e2c025-7d4d-4d90-bd65-05c064c50a5c"), new Guid("47a6853b-a9c7-48a1-8292-5566e754459a") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("e1e2c025-7d4d-4d90-bd65-05c064c50a5c"), new Guid("7eb98e32-d73d-4690-8d00-b79e6dedf00e") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("7eb98e32-d73d-4690-8d00-b79e6dedf00e"), new Guid("9ddd68b5-4a6d-4a26-852e-da44c6323f25") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("47a6853b-a9c7-48a1-8292-5566e754459a"), new Guid("c08e8d07-6ce1-485b-b2eb-fa1896ec55d9") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("7eb98e32-d73d-4690-8d00-b79e6dedf00e"), new Guid("fbe275d8-fa6e-4a0a-9fa5-f3af63a3b9cf") });

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("47a6853b-a9c7-48a1-8292-5566e754459a"));

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("7eb98e32-d73d-4690-8d00-b79e6dedf00e"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("9ddd68b5-4a6d-4a26-852e-da44c6323f25"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("c08e8d07-6ce1-485b-b2eb-fa1896ec55d9"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("fbe275d8-fa6e-4a0a-9fa5-f3af63a3b9cf"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("3646c63b-410a-447c-9019-154d07600fee"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("e1e2c025-7d4d-4d90-bd65-05c064c50a5c"));

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "ImageLocation", "Name" },
                values: new object[,]
                {
                    { new Guid("24b82b49-4347-4bde-a8ff-fa9da3af8a31"), "path/to/image2", "Second Album" },
                    { new Guid("9b05acd7-cca7-48e5-93ea-95c54b7254c4"), "path/to/image1", "First Album" }
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "ImageLocation", "Name" },
                values: new object[,]
                {
                    { new Guid("1915e314-35b8-4492-959e-c8330d32e28d"), "path/to/image2", "Second Artist" },
                    { new Guid("4360f82d-c3d0-4ca9-a2f7-5c5c3c1c8061"), "path/to/image1", "First Artist" }
                });

            migrationBuilder.InsertData(
                table: "AlbumArtistRelations",
                columns: new[] { "AlbumId", "ArtistId" },
                values: new object[,]
                {
                    { new Guid("24b82b49-4347-4bde-a8ff-fa9da3af8a31"), new Guid("1915e314-35b8-4492-959e-c8330d32e28d") },
                    { new Guid("9b05acd7-cca7-48e5-93ea-95c54b7254c4"), new Guid("1915e314-35b8-4492-959e-c8330d32e28d") },
                    { new Guid("9b05acd7-cca7-48e5-93ea-95c54b7254c4"), new Guid("4360f82d-c3d0-4ca9-a2f7-5c5c3c1c8061") }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "AlbumId", "Duration", "PositionInAlbum", "Title" },
                values: new object[,]
                {
                    { new Guid("8769057b-11d7-4188-b15b-545e373c9b36"), new Guid("9b05acd7-cca7-48e5-93ea-95c54b7254c4"), new TimeSpan(0, 0, 3, 45, 0), -1, "First Song" },
                    { new Guid("ab1835db-c7b4-4b92-bcc1-ea88c6867868"), new Guid("24b82b49-4347-4bde-a8ff-fa9da3af8a31"), new TimeSpan(0, 0, 5, 0, 0), -1, "Third Song" },
                    { new Guid("cc92ed37-447b-4563-8f56-4c52e2c3867b"), new Guid("9b05acd7-cca7-48e5-93ea-95c54b7254c4"), new TimeSpan(0, 0, 4, 20, 0), -1, "Second Song" }
                });

            migrationBuilder.InsertData(
                table: "SongArtistRelations",
                columns: new[] { "ArtistId", "SongId" },
                values: new object[,]
                {
                    { new Guid("4360f82d-c3d0-4ca9-a2f7-5c5c3c1c8061"), new Guid("8769057b-11d7-4188-b15b-545e373c9b36") },
                    { new Guid("1915e314-35b8-4492-959e-c8330d32e28d"), new Guid("ab1835db-c7b4-4b92-bcc1-ea88c6867868") },
                    { new Guid("4360f82d-c3d0-4ca9-a2f7-5c5c3c1c8061"), new Guid("cc92ed37-447b-4563-8f56-4c52e2c3867b") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("24b82b49-4347-4bde-a8ff-fa9da3af8a31"), new Guid("1915e314-35b8-4492-959e-c8330d32e28d") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("9b05acd7-cca7-48e5-93ea-95c54b7254c4"), new Guid("1915e314-35b8-4492-959e-c8330d32e28d") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("9b05acd7-cca7-48e5-93ea-95c54b7254c4"), new Guid("4360f82d-c3d0-4ca9-a2f7-5c5c3c1c8061") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("4360f82d-c3d0-4ca9-a2f7-5c5c3c1c8061"), new Guid("8769057b-11d7-4188-b15b-545e373c9b36") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("1915e314-35b8-4492-959e-c8330d32e28d"), new Guid("ab1835db-c7b4-4b92-bcc1-ea88c6867868") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("4360f82d-c3d0-4ca9-a2f7-5c5c3c1c8061"), new Guid("cc92ed37-447b-4563-8f56-4c52e2c3867b") });

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("1915e314-35b8-4492-959e-c8330d32e28d"));

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("4360f82d-c3d0-4ca9-a2f7-5c5c3c1c8061"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("8769057b-11d7-4188-b15b-545e373c9b36"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("ab1835db-c7b4-4b92-bcc1-ea88c6867868"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("cc92ed37-447b-4563-8f56-4c52e2c3867b"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("24b82b49-4347-4bde-a8ff-fa9da3af8a31"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("9b05acd7-cca7-48e5-93ea-95c54b7254c4"));

            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "ImageLocation", "Name" },
                values: new object[,]
                {
                    { new Guid("3646c63b-410a-447c-9019-154d07600fee"), "path/to/image2", "Second Album" },
                    { new Guid("e1e2c025-7d4d-4d90-bd65-05c064c50a5c"), "path/to/image1", "First Album" }
                });

            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "Id", "ImageLocation", "Name" },
                values: new object[,]
                {
                    { new Guid("47a6853b-a9c7-48a1-8292-5566e754459a"), "path/to/image2", "Second Artist" },
                    { new Guid("7eb98e32-d73d-4690-8d00-b79e6dedf00e"), "path/to/image1", "First Artist" }
                });

            migrationBuilder.InsertData(
                table: "AlbumArtistRelations",
                columns: new[] { "AlbumId", "ArtistId" },
                values: new object[,]
                {
                    { new Guid("3646c63b-410a-447c-9019-154d07600fee"), new Guid("47a6853b-a9c7-48a1-8292-5566e754459a") },
                    { new Guid("e1e2c025-7d4d-4d90-bd65-05c064c50a5c"), new Guid("47a6853b-a9c7-48a1-8292-5566e754459a") },
                    { new Guid("e1e2c025-7d4d-4d90-bd65-05c064c50a5c"), new Guid("7eb98e32-d73d-4690-8d00-b79e6dedf00e") }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "AlbumId", "Duration", "PositionInAlbum", "Title" },
                values: new object[,]
                {
                    { new Guid("9ddd68b5-4a6d-4a26-852e-da44c6323f25"), new Guid("e1e2c025-7d4d-4d90-bd65-05c064c50a5c"), new TimeSpan(0, 0, 3, 45, 0), -1, "First Song" },
                    { new Guid("c08e8d07-6ce1-485b-b2eb-fa1896ec55d9"), new Guid("3646c63b-410a-447c-9019-154d07600fee"), new TimeSpan(0, 0, 5, 0, 0), -1, "Third Song" },
                    { new Guid("fbe275d8-fa6e-4a0a-9fa5-f3af63a3b9cf"), new Guid("e1e2c025-7d4d-4d90-bd65-05c064c50a5c"), new TimeSpan(0, 0, 4, 20, 0), -1, "Second Song" }
                });

            migrationBuilder.InsertData(
                table: "SongArtistRelations",
                columns: new[] { "ArtistId", "SongId" },
                values: new object[,]
                {
                    { new Guid("7eb98e32-d73d-4690-8d00-b79e6dedf00e"), new Guid("9ddd68b5-4a6d-4a26-852e-da44c6323f25") },
                    { new Guid("47a6853b-a9c7-48a1-8292-5566e754459a"), new Guid("c08e8d07-6ce1-485b-b2eb-fa1896ec55d9") },
                    { new Guid("7eb98e32-d73d-4690-8d00-b79e6dedf00e"), new Guid("fbe275d8-fa6e-4a0a-9fa5-f3af63a3b9cf") }
                });
        }
    }
}
