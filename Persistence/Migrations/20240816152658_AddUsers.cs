using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("70a957f6-ed66-48de-b72c-47224dc5d70a"), new Guid("4bab74a2-a8d1-483b-bb89-4eaffb704dd5") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("70a957f6-ed66-48de-b72c-47224dc5d70a"), new Guid("5890e712-b6ce-47e3-a5c3-2ee41bbc3651") });

            migrationBuilder.DeleteData(
                table: "AlbumArtistRelations",
                keyColumns: new[] { "AlbumId", "ArtistId" },
                keyValues: new object[] { new Guid("8cc948dd-374a-47a2-aba6-52d02151e14b"), new Guid("4bab74a2-a8d1-483b-bb89-4eaffb704dd5") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("5890e712-b6ce-47e3-a5c3-2ee41bbc3651"), new Guid("39fe0f6f-d521-446c-925c-5887b8c11b97") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("4bab74a2-a8d1-483b-bb89-4eaffb704dd5"), new Guid("477b3f00-7d39-4472-a79e-b21b48aa1dc0") });

            migrationBuilder.DeleteData(
                table: "SongArtistRelations",
                keyColumns: new[] { "ArtistId", "SongId" },
                keyValues: new object[] { new Guid("5890e712-b6ce-47e3-a5c3-2ee41bbc3651"), new Guid("ff72bbf1-75d1-42a4-8f57-83f2392e63e7") });

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("4bab74a2-a8d1-483b-bb89-4eaffb704dd5"));

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "Id",
                keyValue: new Guid("5890e712-b6ce-47e3-a5c3-2ee41bbc3651"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("39fe0f6f-d521-446c-925c-5887b8c11b97"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("477b3f00-7d39-4472-a79e-b21b48aa1dc0"));

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: new Guid("ff72bbf1-75d1-42a4-8f57-83f2392e63e7"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("70a957f6-ed66-48de-b72c-47224dc5d70a"));

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: new Guid("8cc948dd-374a-47a2-aba6-52d02151e14b"));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, defaultValueSql: "'UUID()'", collation: "ascii_general_ci"),
                    UserName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

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
        }
    }
}
