﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Persistence;

#nullable disable

namespace Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Domain.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ImageLocation")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .HasDatabaseName("IX_Album_Name");

                    b.ToTable("Albums", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ImageLocation = "path/to/image1",
                            Name = "First Album"
                        },
                        new
                        {
                            Id = 2,
                            ImageLocation = "path/to/image2",
                            Name = "Second Album"
                        });
                });

            modelBuilder.Entity("Domain.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ImageLocation")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .HasDatabaseName("IX_Artist_Name");

                    b.ToTable("Artists", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ImageLocation = "path/to/image1",
                            Name = "First Artist"
                        },
                        new
                        {
                            Id = 2,
                            ImageLocation = "path/to/image2",
                            Name = "Second Artist"
                        });
                });

            modelBuilder.Entity("Domain.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AlbumId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time(6)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("Title")
                        .HasDatabaseName("IX_Song_Title");

                    b.ToTable("Songs", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AlbumId = 1,
                            Duration = new TimeSpan(0, 0, 3, 45, 0),
                            Title = "First Song"
                        },
                        new
                        {
                            Id = 2,
                            AlbumId = 1,
                            Duration = new TimeSpan(0, 0, 4, 20, 0),
                            Title = "Second Song"
                        },
                        new
                        {
                            Id = 3,
                            AlbumId = 2,
                            Duration = new TimeSpan(0, 0, 5, 0, 0),
                            Title = "Third Song"
                        });
                });

            modelBuilder.Entity("Domain.SongArtistRelation", b =>
                {
                    b.Property<int>("SongId")
                        .HasColumnType("int");

                    b.Property<int>("ArtistId")
                        .HasColumnType("int");

                    b.HasKey("SongId", "ArtistId");

                    b.HasIndex("ArtistId");

                    b.ToTable("SongArtistRelations");

                    b.HasData(
                        new
                        {
                            SongId = 1,
                            ArtistId = 1
                        },
                        new
                        {
                            SongId = 2,
                            ArtistId = 1
                        },
                        new
                        {
                            SongId = 3,
                            ArtistId = 2
                        });
                });

            modelBuilder.Entity("Domain.Song", b =>
                {
                    b.HasOne("Domain.Album", "Album")
                        .WithMany("Songs")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Album");
                });

            modelBuilder.Entity("Domain.SongArtistRelation", b =>
                {
                    b.HasOne("Domain.Artist", "Artist")
                        .WithMany("SongArtistRelations")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Song", "Song")
                        .WithMany("SongArtistRelations")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("Domain.Album", b =>
                {
                    b.Navigation("Songs");
                });

            modelBuilder.Entity("Domain.Artist", b =>
                {
                    b.Navigation("SongArtistRelations");
                });

            modelBuilder.Entity("Domain.Song", b =>
                {
                    b.Navigation("SongArtistRelations");
                });
#pragma warning restore 612, 618
        }
    }
}
