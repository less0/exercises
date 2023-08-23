﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using bowling_backend_persistence;

#nullable disable

namespace bowling_backend_persistence.Migrations
{
    [DbContext(typeof(BowlingDataContext))]
    [Migration("20230823151310_AddUserId")]
    partial class AddUserId
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("bowling_backend_persistence.DataModel.BowlingGame", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PlayerNames")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Id", "UserId");

                    b.ToTable("BowlingGame");
                });

            modelBuilder.Entity("bowling_backend_persistence.DataModel.Frame", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("BonusPoints")
                        .HasColumnType("int");

                    b.Property<Guid?>("BowlingGameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsLastFrame")
                        .HasColumnType("bit");

                    b.Property<int>("PlayerIndex")
                        .HasColumnType("int");

                    b.Property<string>("Rolls")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BowlingGameId");

                    b.ToTable("Frame");
                });

            modelBuilder.Entity("bowling_backend_persistence.DataModel.Frame", b =>
                {
                    b.HasOne("bowling_backend_persistence.DataModel.BowlingGame", null)
                        .WithMany("Frames")
                        .HasForeignKey("BowlingGameId");
                });

            modelBuilder.Entity("bowling_backend_persistence.DataModel.BowlingGame", b =>
                {
                    b.Navigation("Frames");
                });
#pragma warning restore 612, 618
        }
    }
}
