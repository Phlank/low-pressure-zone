﻿// <auto-generated />
using System;
using System.Collections.Generic;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LowPressureZone.Domain.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LowPressureZone.Domain.Entities.Audience", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.PrimitiveCollection<List<Guid>>("LinkedUserIds")
                        .IsRequired()
                        .HasColumnType("uuid[]");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Audiences");
                });

            modelBuilder.Entity("LowPressureZone.Domain.Entities.Performer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.PrimitiveCollection<List<Guid>>("LinkedUserIds")
                        .IsRequired()
                        .HasColumnType("uuid[]");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Performers");
                });

            modelBuilder.Entity("LowPressureZone.Domain.Entities.Schedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AudienceId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("EndsAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("StartsAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AudienceId");

                    b.HasIndex("EndsAt")
                        .IsUnique();

                    b.HasIndex("StartsAt")
                        .IsUnique();

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("LowPressureZone.Domain.Entities.Timeslot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("EndsAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<Guid>("PerformerId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ScheduleId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("StartsAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EndsAt")
                        .IsUnique();

                    b.HasIndex("PerformerId");

                    b.HasIndex("ScheduleId");

                    b.HasIndex("StartsAt")
                        .IsUnique();

                    b.ToTable("Timeslots");
                });

            modelBuilder.Entity("LowPressureZone.Domain.Entities.Schedule", b =>
                {
                    b.HasOne("LowPressureZone.Domain.Entities.Audience", "Audience")
                        .WithMany("Schedules")
                        .HasForeignKey("AudienceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Audience");
                });

            modelBuilder.Entity("LowPressureZone.Domain.Entities.Timeslot", b =>
                {
                    b.HasOne("LowPressureZone.Domain.Entities.Performer", "Performer")
                        .WithMany("Timeslots")
                        .HasForeignKey("PerformerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LowPressureZone.Domain.Entities.Schedule", "Schedule")
                        .WithMany("Timeslots")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Performer");

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("LowPressureZone.Domain.Entities.Audience", b =>
                {
                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("LowPressureZone.Domain.Entities.Performer", b =>
                {
                    b.Navigation("Timeslots");
                });

            modelBuilder.Entity("LowPressureZone.Domain.Entities.Schedule", b =>
                {
                    b.Navigation("Timeslots");
                });
#pragma warning restore 612, 618
        }
    }
}
