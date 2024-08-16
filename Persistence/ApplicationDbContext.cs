using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<SongArtistRelation> SongArtistRelations { get; set; }
        public DbSet<AlbumArtistRelation> AlbumArtistRelations { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Artist>().ToTable("Artists");

            modelBuilder.Entity<Song>()
                .Property(s => s.Id)
                .HasDefaultValueSql("'UUID()'");

            modelBuilder.Entity<Album>()
                .Property(a => a.Id)
                .HasDefaultValueSql("'UUID()'");

            modelBuilder.Entity<Artist>()
                .Property(art => art.Id)
                .HasDefaultValueSql("'UUID()'");

            modelBuilder.Entity<Song>()
                .ToTable("Songs")
                .HasIndex(song => song.Title)
                .HasDatabaseName("IX_Song_Title");

            modelBuilder.Entity<Album>()
                .ToTable("Albums")
                .HasIndex(album => album.Name)
                .HasDatabaseName("IX_Album_Name");
            modelBuilder.Entity<Artist>()
                .ToTable("Artists")
                .HasIndex(artist => artist.Name)
                .HasDatabaseName("IX_Artist_Name");


            modelBuilder.Entity<SongArtistRelation>()
                .HasIndex(sar => sar.SongId)
                .HasDatabaseName("IX_SongArtistRelation_SongId");

            modelBuilder.Entity<SongArtistRelation>()
                .HasIndex(sar => sar.ArtistId)
                .HasDatabaseName("IX_SongArtistRelation_ArtistId");

            modelBuilder.Entity<AlbumArtistRelation>()
                .HasIndex(aar => aar.AlbumId)
                .HasDatabaseName("IX_AlbumArtistRelation_AlbumId");

            modelBuilder.Entity<AlbumArtistRelation>()
                .HasIndex(aar => aar.ArtistId)
                .HasDatabaseName("IX_AlbumArtistRelation_ArtistId");

            modelBuilder.Entity<Song>()
                .HasIndex(s => new { s.Title, s.AlbumId })
                .HasDatabaseName("IX_Song_Title_AlbumId");

            modelBuilder.Entity<Song>()
                .HasIndex(s => s.AlbumId)
                .HasDatabaseName("IX_Song_AlbumId");

            //One to many relation between Album and Song
            modelBuilder.Entity<Song>()
                .HasOne(song => song.Album)
                .WithMany(album => album.Songs)
                .HasForeignKey(s => s.AlbumId)
                //If related foreign key album is deleted, song should also be deleted.
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SongArtistRelation>()
                .HasKey(sar => new { sar.SongId, sar.ArtistId });

            modelBuilder.Entity<SongArtistRelation>()
                .HasOne(sar => sar.Song)
                .WithMany(song => song.SongArtistRelations)
                .HasForeignKey(sar => sar.SongId);

            modelBuilder.Entity<SongArtistRelation>()
                .HasOne(sar => sar.Artist)
                .WithMany(artist => artist.SongArtistRelations)
                .HasForeignKey(sar => sar.ArtistId);

            modelBuilder.Entity<AlbumArtistRelation>()
                .HasKey(aar => new { aar.AlbumId, aar.ArtistId });

            modelBuilder.Entity<AlbumArtistRelation>()
                .HasOne(aar => aar.Album)
                .WithMany(album => album.AlbumArtistRelations)
                .HasForeignKey(aar => aar.AlbumId);

            modelBuilder.Entity<AlbumArtistRelation>()
                .HasOne(aar => aar.Artist)
                .WithMany(artist => artist.AlbumArtistRelations)
                .HasForeignKey(aar => aar.ArtistId);

            // GUIDs for Albums
            var album1Id = Guid.NewGuid();
            var album2Id = Guid.NewGuid();

            // GUIDs for Artists
            var artist1Id = Guid.NewGuid();
            var artist2Id = Guid.NewGuid();

            // GUIDs for Songs
            var song1Id = Guid.NewGuid();
            var song2Id = Guid.NewGuid();
            var song3Id = Guid.NewGuid();

            // Seed Data for Albums
            modelBuilder.Entity<Album>().HasData(
                new Album { Id = album1Id, Name = "First Album", ImageLocation = "path/to/image1" },
                new Album { Id = album2Id, Name = "Second Album", ImageLocation = "path/to/image2" }
            );

            // Seed Data for Artists
            modelBuilder.Entity<Artist>().HasData(
                new Artist { Id = artist1Id, Name = "First Artist", ImageLocation = "path/to/image1" },
                new Artist { Id = artist2Id, Name = "Second Artist", ImageLocation = "path/to/image2" }
            );

            // Seed Data for Songs
            modelBuilder.Entity<Song>().HasData(
                new Song { Id = song1Id, Title = "First Song", Duration = new TimeSpan(0, 3, 45), AlbumId = album1Id },
                new Song { Id = song2Id, Title = "Second Song", Duration = new TimeSpan(0, 4, 20), AlbumId = album1Id },
                new Song { Id = song3Id, Title = "Third Song", Duration = new TimeSpan(0, 5, 0), AlbumId = album2Id }
            );

            // Seed Data for SongArtistRelation
            modelBuilder.Entity<SongArtistRelation>().HasData(
                new SongArtistRelation { SongId = song1Id, ArtistId = artist1Id },
                new SongArtistRelation { SongId = song2Id, ArtistId = artist1Id },
                new SongArtistRelation { SongId = song3Id, ArtistId = artist2Id }
            );

            // Seed Data for AlbumArtistRelation
            modelBuilder.Entity<AlbumArtistRelation>().HasData(
                new AlbumArtistRelation { AlbumId = album1Id, ArtistId = artist1Id },
                new AlbumArtistRelation { AlbumId = album1Id, ArtistId = artist2Id },
                new AlbumArtistRelation { AlbumId = album2Id, ArtistId = artist2Id }
            );
        }
    }
}