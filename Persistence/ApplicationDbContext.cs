using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ImageAccent> ImageAccents { get; set; }
        public DbSet<SongArtistRelation> SongArtistRelations { get; set; }
        public DbSet<AlbumArtistRelation> AlbumArtistRelations { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSongRelation> PlaylistSongRelations { get; set; }
        public DbSet<PlayRecord> PlayRecords { get; set; }
        public DbSet<SongPlayRecord> SongPlayRecords { get; set; }
        public DbSet<AlbumPlayRecord> AlbumPlayRecords { get; set; }
        public DbSet<PlaylistPlayRecord> PlaylistPlayRecords { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            UpdateTimeStamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimeStamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimeStamps()
        {
            var timeStampedEntities = new HashSet<Type>
            {
                typeof(Artist),
                typeof(Album),
                typeof(Song),
                typeof(User),
                typeof(Playlist)
            };

            var entries = ChangeTracker.Entries()
                .Where(e => timeStampedEntities.Contains(e.Entity.GetType()) && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    SetPropertyValue(entry.Entity, "CreatedAt", DateTime.UtcNow);
                }

                SetPropertyValue(entry.Entity, "UpdatedAt", DateTime.UtcNow);
            }
        }

        private void SetPropertyValue(object entity, string propertyName, object value)
        {
            var property = entity.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                property.SetValue(entity, value);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ImageAccent>()
                .HasKey(i => i.ImagePath);

            modelBuilder.Entity<Artist>().ToTable("Artists");

            modelBuilder.Entity<Song>()
                .Property(s => s.Id)
                .HasDefaultValueSql("'UUID()'");

            modelBuilder.Entity<Album>()
                .Property(a => a.Id)
                .HasDefaultValueSql("'UUID()'");

            modelBuilder.Entity<Playlist>()
                .Property(p => p.Id)
                .HasDefaultValueSql("'UUID()'");

            modelBuilder.Entity<Playlist>()
                .Property(p => p.Visibility)
                .HasConversion<string>()
                .HasColumnType("varchar(10)")
                .HasColumnName("Visibility");

            modelBuilder.Entity<Artist>()
                .Property(art => art.Id)
                .HasDefaultValueSql("'UUID()'");

            modelBuilder.Entity<User>()
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

            modelBuilder.Entity<PlaylistSongRelation>()
                .HasIndex(psr => psr.PlaylistId)
                .HasDatabaseName("IX_PlaylistSongRelation_PlaylistId");

            modelBuilder.Entity<PlaylistSongRelation>()
                .HasIndex(psr => psr.SongId)
                .HasDatabaseName("IX_PlaylistSongRelation_SongId");

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

            modelBuilder.Entity<Album>()
        .HasIndex(album => album.CreatedAt)
        .HasDatabaseName("IX_Album_CreatedAt");

            modelBuilder.Entity<Album>()
                .HasIndex(album => album.UpdatedAt)
                .HasDatabaseName("IX_Album_UpdatedAt");

            modelBuilder.Entity<Artist>()
                .HasIndex(artist => artist.CreatedAt)
                .HasDatabaseName("IX_Artist_CreatedAt");

            modelBuilder.Entity<Artist>()
                .HasIndex(artist => artist.UpdatedAt)
                .HasDatabaseName("IX_Artist_UpdatedAt");

            modelBuilder.Entity<Song>()
                .HasIndex(song => song.CreatedAt)
                .HasDatabaseName("IX_Song_CreatedAt");

            modelBuilder.Entity<Song>()
                .HasIndex(song => song.UpdatedAt)
                .HasDatabaseName("IX_Song_UpdatedAt");

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

            modelBuilder.Entity<PlaylistSongRelation>()
                .HasKey(psr => new { psr.PlaylistId, psr.SongId });

            modelBuilder.Entity<AlbumArtistRelation>()
                .HasOne(aar => aar.Album)
                .WithMany(album => album.AlbumArtistRelations)
                .HasForeignKey(aar => aar.AlbumId);

            modelBuilder.Entity<AlbumArtistRelation>()
                .HasOne(aar => aar.Artist)
                .WithMany(artist => artist.AlbumArtistRelations)
                .HasForeignKey(aar => aar.ArtistId);

            modelBuilder.Entity<PlaylistSongRelation>()
                .HasOne(psr => psr.Song)
                .WithMany(song => song.PlaylistSongRelations)
                .HasForeignKey(psr => psr.SongId);

            modelBuilder.Entity<PlaylistSongRelation>()
                .HasOne(psr => psr.Playlist)
                .WithMany(playlist => playlist.PlaylistSongRelations)
                .HasForeignKey(psr => psr.PlaylistId);

            modelBuilder.Entity<PlayRecord>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<PlayRecord>()
                .HasDiscriminator(p => p.PlayedItemType)
                .HasValue<SongPlayRecord>(PlayedItemType.Song)
                .HasValue<AlbumPlayRecord>(PlayedItemType.Album)
                .HasValue<PlaylistPlayRecord>(PlayedItemType.Playlist);

            modelBuilder.Entity<PlayRecord>()
                .HasIndex(p => new { p.PlayedItemType, p.PlayedItemId });

            modelBuilder.Entity<PlayRecord>()
                .HasIndex(p => p.Timestamp);

            modelBuilder.Entity<PlayRecord>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<PlayRecord>()
                .Property(p => p.Id)
                .HasColumnName("Id");

            modelBuilder.Entity<PlayRecord>()
                .Property(p => p.PlayedItemType)
                .HasConversion<string>()
                .HasColumnName("PlayedItemType");

            modelBuilder.Entity<PlayRecord>()
                .Property(p => p.PlayedItemId)
                .HasColumnName("PlayedItemId");

            modelBuilder.Entity<PlayRecord>()
                .Property(p => p.UserId)
                .HasColumnName("UserId");

            modelBuilder.Entity<PlayRecord>()
                .Property(p => p.Timestamp)
                .HasColumnName("Timestamp");
        }
    }
}