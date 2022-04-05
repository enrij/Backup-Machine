﻿// <auto-generated />
using System;
using BackupMachine.PoC.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BackupMachine.PoC.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(BackupMachineContext))]
    partial class BackupMachineContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.3");

            modelBuilder.Entity("BackupMachine.PoC.Domain.Entities.Backup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("JobId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("PreviousBackupId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("JobId");

                    b.HasIndex("PreviousBackupId");

                    b.ToTable("Backups", (string)null);
                });

            modelBuilder.Entity("BackupMachine.PoC.Domain.Entities.BackupFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BackupFolderId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BackupId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BackupFolderId");

                    b.HasIndex("BackupId");

                    b.ToTable("Files", (string)null);
                });

            modelBuilder.Entity("BackupMachine.PoC.Domain.Entities.BackupFolder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BackupId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ParentFolderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RelativePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BackupId");

                    b.HasIndex("ParentFolderId");

                    b.ToTable("Folders", (string)null);
                });

            modelBuilder.Entity("BackupMachine.PoC.Domain.Entities.Job", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Destination")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Jobs", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("e005279a-2b23-4a3c-b798-27cb443daf9e"),
                            Destination = "C:\\Temp\\Backups\\SmallSource",
                            Name = "Test",
                            Source = "C:\\Temp\\Sources\\SmallSource"
                        });
                });

            modelBuilder.Entity("BackupMachine.PoC.Domain.Entities.Backup", b =>
                {
                    b.HasOne("BackupMachine.PoC.Domain.Entities.Job", "Job")
                        .WithMany("Backups")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BackupMachine.PoC.Domain.Entities.Backup", "PreviousBackup")
                        .WithMany()
                        .HasForeignKey("PreviousBackupId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Job");

                    b.Navigation("PreviousBackup");
                });

            modelBuilder.Entity("BackupMachine.PoC.Domain.Entities.BackupFile", b =>
                {
                    b.HasOne("BackupMachine.PoC.Domain.Entities.BackupFolder", "BackupFolder")
                        .WithMany("Files")
                        .HasForeignKey("BackupFolderId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BackupMachine.PoC.Domain.Entities.Backup", "Backup")
                        .WithMany()
                        .HasForeignKey("BackupId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Backup");

                    b.Navigation("BackupFolder");
                });

            modelBuilder.Entity("BackupMachine.PoC.Domain.Entities.BackupFolder", b =>
                {
                    b.HasOne("BackupMachine.PoC.Domain.Entities.Backup", "Backup")
                        .WithMany()
                        .HasForeignKey("BackupId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BackupMachine.PoC.Domain.Entities.BackupFolder", "ParentFolder")
                        .WithMany("Subfolders")
                        .HasForeignKey("ParentFolderId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Backup");

                    b.Navigation("ParentFolder");
                });

            modelBuilder.Entity("BackupMachine.PoC.Domain.Entities.BackupFolder", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("Subfolders");
                });

            modelBuilder.Entity("BackupMachine.PoC.Domain.Entities.Job", b =>
                {
                    b.Navigation("Backups");
                });
#pragma warning restore 612, 618
        }
    }
}
