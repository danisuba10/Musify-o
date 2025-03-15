using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRestrictingCascadeForAlbumAndArtist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumArtistRelations_Albums_AlbumId",
                table: "AlbumArtistRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_SongArtistRelations_Songs_SongId",
                table: "SongArtistRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Albums_AlbumId",
                table: "Songs");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumArtistRelations_Albums_AlbumId",
                table: "AlbumArtistRelations",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SongArtistRelations_Songs_SongId",
                table: "SongArtistRelations",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Albums_AlbumId",
                table: "Songs",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumArtistRelations_Albums_AlbumId",
                table: "AlbumArtistRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_SongArtistRelations_Songs_SongId",
                table: "SongArtistRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Albums_AlbumId",
                table: "Songs");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumArtistRelations_Albums_AlbumId",
                table: "AlbumArtistRelations",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongArtistRelations_Songs_SongId",
                table: "SongArtistRelations",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Albums_AlbumId",
                table: "Songs",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
