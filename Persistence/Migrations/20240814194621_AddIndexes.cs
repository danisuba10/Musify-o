using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Albums_AlbumId",
                table: "Songs");

            migrationBuilder.RenameIndex(
                name: "IX_Songs_AlbumId",
                table: "Songs",
                newName: "IX_Song_AlbumId");

            migrationBuilder.RenameIndex(
                name: "IX_SongArtistRelations_ArtistId",
                table: "SongArtistRelations",
                newName: "IX_SongArtistRelation_ArtistId");

            migrationBuilder.RenameIndex(
                name: "IX_AlbumArtistRelations_ArtistId",
                table: "AlbumArtistRelations",
                newName: "IX_AlbumArtistRelation_ArtistId");

            migrationBuilder.AlterColumn<int>(
                name: "AlbumId",
                table: "Songs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PositionInAlbum",
                table: "Songs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 1,
                column: "PositionInAlbum",
                value: -1);

            migrationBuilder.UpdateData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 2,
                column: "PositionInAlbum",
                value: -1);

            migrationBuilder.UpdateData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 3,
                column: "PositionInAlbum",
                value: -1);

            migrationBuilder.CreateIndex(
                name: "IX_Song_Title_AlbumId",
                table: "Songs",
                columns: new[] { "Title", "AlbumId" });

            migrationBuilder.CreateIndex(
                name: "IX_SongArtistRelation_SongId",
                table: "SongArtistRelations",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumArtistRelation_AlbumId",
                table: "AlbumArtistRelations",
                column: "AlbumId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Albums_AlbumId",
                table: "Songs",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Albums_AlbumId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Song_Title_AlbumId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_SongArtistRelation_SongId",
                table: "SongArtistRelations");

            migrationBuilder.DropIndex(
                name: "IX_AlbumArtistRelation_AlbumId",
                table: "AlbumArtistRelations");

            migrationBuilder.DropColumn(
                name: "PositionInAlbum",
                table: "Songs");

            migrationBuilder.RenameIndex(
                name: "IX_Song_AlbumId",
                table: "Songs",
                newName: "IX_Songs_AlbumId");

            migrationBuilder.RenameIndex(
                name: "IX_SongArtistRelation_ArtistId",
                table: "SongArtistRelations",
                newName: "IX_SongArtistRelations_ArtistId");

            migrationBuilder.RenameIndex(
                name: "IX_AlbumArtistRelation_ArtistId",
                table: "AlbumArtistRelations",
                newName: "IX_AlbumArtistRelations_ArtistId");

            migrationBuilder.AlterColumn<int>(
                name: "AlbumId",
                table: "Songs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Albums_AlbumId",
                table: "Songs",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
