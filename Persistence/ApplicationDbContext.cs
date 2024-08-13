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

            modelBuilder.Entity<Album>().HasData(
                new Album { Id = 1, Name = "First Album", ImageLocation = "path/to/image1" },
                new Album { Id = 2, Name = "Second Album", ImageLocation = "path/to/image2" }
            );

            modelBuilder.Entity<Artist>().HasData(
                new Artist { Id = 1, Name = "First Artist", ImageLocation = "path/to/image1" },
                new Artist { Id = 2, Name = "Second Artist", ImageLocation = "path/to/image2" }
            );

            modelBuilder.Entity<Song>().HasData(
                new Song { Id = 1, Title = "First Song", Duration = new TimeSpan(0, 3, 45), AlbumId = 1 },
                new Song { Id = 2, Title = "Second Song", Duration = new TimeSpan(0, 4, 20), AlbumId = 1 },
                new Song { Id = 3, Title = "Third Song", Duration = new TimeSpan(0, 5, 0), AlbumId = 2 }
            );

            modelBuilder.Entity<SongArtistRelation>().HasData(
                new SongArtistRelation { SongId = 1, ArtistId = 1 },
                new SongArtistRelation { SongId = 2, ArtistId = 1 },
                new SongArtistRelation { SongId = 3, ArtistId = 2 }
            );

            modelBuilder.Entity<AlbumArtistRelation>().HasData(
                new AlbumArtistRelation { AlbumId = 1, ArtistId = 1 },
                new AlbumArtistRelation { AlbumId = 1, ArtistId = 2 },
                new AlbumArtistRelation { AlbumId = 2, ArtistId = 2 }
            );
        }
    }
}