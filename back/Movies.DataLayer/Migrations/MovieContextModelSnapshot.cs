﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Movies.DataLayer;

#nullable disable

namespace Movies.DataLayer.Migrations
{
    [DbContext(typeof(MovieContext))]
    partial class MovieContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Movies.Domain.Entities.CinemaPeople", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int?>("BirthYear")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("CinemaPeople");
                });

            modelBuilder.Entity("Movies.Domain.Entities.Comment", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<decimal>("MovieId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("UserId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Movies.Domain.Entities.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("Movies.Domain.Entities.Movie", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("Movies.Domain.Entities.MovieActor", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<decimal>("CinemaPeopleId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<decimal>("MovieId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("CinemaPeopleId");

                    b.HasIndex("MovieId");

                    b.ToTable("MoviesActors");
                });

            modelBuilder.Entity("Movies.Domain.Entities.MovieDirector", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<decimal>("CinemaPeopleId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<decimal>("MovieId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("CinemaPeopleId");

                    b.HasIndex("MovieId");

                    b.ToTable("MoviesDirectors");
                });

            modelBuilder.Entity("Movies.Domain.Entities.MovieGenre", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<decimal>("MovieId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("GenreId");

                    b.HasIndex("MovieId");

                    b.ToTable("MoviesGenres");
                });

            modelBuilder.Entity("Movies.Domain.Entities.User", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Movies.Domain.Entities.UserRate", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<decimal>("MovieId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<byte>("Score")
                        .HasColumnType("tinyint");

                    b.Property<decimal>("UserId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersRates");
                });

            modelBuilder.Entity("Movies.Domain.Entities.Watchlist", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<decimal>("UserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("WatchListName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Watchlists");
                });

            modelBuilder.Entity("Movies.Domain.Entities.WatchlistMovie", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<decimal>("MovieId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("WatchlistId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("MovieId");

                    b.HasIndex("WatchlistId");

                    b.ToTable("WatchlistsMovies");
                });

            modelBuilder.Entity("Movies.Domain.Entities.Comment", b =>
                {
                    b.HasOne("Movies.Domain.Entities.Movie", "Movie")
                        .WithMany("Comments")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Movies.Domain.Entities.User", "User")
                        .WithMany("UserComments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Movies.Domain.Entities.MovieActor", b =>
                {
                    b.HasOne("Movies.Domain.Entities.CinemaPeople", "CinemaPeople")
                        .WithMany("ActorMovies")
                        .HasForeignKey("CinemaPeopleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Movies.Domain.Entities.Movie", "Movie")
                        .WithMany("MovieActors")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CinemaPeople");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("Movies.Domain.Entities.MovieDirector", b =>
                {
                    b.HasOne("Movies.Domain.Entities.CinemaPeople", "CinemaPeople")
                        .WithMany("DirectorMovies")
                        .HasForeignKey("CinemaPeopleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Movies.Domain.Entities.Movie", "Movie")
                        .WithMany("MovieDirectors")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CinemaPeople");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("Movies.Domain.Entities.MovieGenre", b =>
                {
                    b.HasOne("Movies.Domain.Entities.Genre", "Genre")
                        .WithMany("Movies")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Movies.Domain.Entities.Movie", "Movie")
                        .WithMany("Genres")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Genre");

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("Movies.Domain.Entities.UserRate", b =>
                {
                    b.HasOne("Movies.Domain.Entities.Movie", "Movie")
                        .WithMany("Rates")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Movies.Domain.Entities.User", "User")
                        .WithMany("UserRates")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Movies.Domain.Entities.Watchlist", b =>
                {
                    b.HasOne("Movies.Domain.Entities.User", "User")
                        .WithMany("Watchlists")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Movies.Domain.Entities.WatchlistMovie", b =>
                {
                    b.HasOne("Movies.Domain.Entities.Movie", "Movie")
                        .WithMany("WatchlistMovies")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Movies.Domain.Entities.Watchlist", "Watchlist")
                        .WithMany("WatchlistMovies")
                        .HasForeignKey("WatchlistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");

                    b.Navigation("Watchlist");
                });

            modelBuilder.Entity("Movies.Domain.Entities.CinemaPeople", b =>
                {
                    b.Navigation("ActorMovies");

                    b.Navigation("DirectorMovies");
                });

            modelBuilder.Entity("Movies.Domain.Entities.Genre", b =>
                {
                    b.Navigation("Movies");
                });

            modelBuilder.Entity("Movies.Domain.Entities.Movie", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Genres");

                    b.Navigation("MovieActors");

                    b.Navigation("MovieDirectors");

                    b.Navigation("Rates");

                    b.Navigation("WatchlistMovies");
                });

            modelBuilder.Entity("Movies.Domain.Entities.User", b =>
                {
                    b.Navigation("UserComments");

                    b.Navigation("UserRates");

                    b.Navigation("Watchlists");
                });

            modelBuilder.Entity("Movies.Domain.Entities.Watchlist", b =>
                {
                    b.Navigation("WatchlistMovies");
                });
#pragma warning restore 612, 618
        }
    }
}
