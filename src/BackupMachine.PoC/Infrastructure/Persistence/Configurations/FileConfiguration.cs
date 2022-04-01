using BackupMachine.PoC.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackupMachine.PoC.Infrastructure.Persistence.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<FileCopy>
{
    public void Configure(EntityTypeBuilder<FileCopy> builder)
    {
        builder.ToTable("Files");

        builder.HasKey(file => file.Id);

        builder.Property(file => file.Id)
               .ValueGeneratedOnAdd();

        builder.HasOne(file => file.Snapshot)
               .WithMany()
               .HasForeignKey(file => file.SnapshotId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(file => file.PreviousVersion)
               .WithOne()
               .HasForeignKey<FileCopy>(previous => previous.PreviousVersionId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(file => file.Folder)
               .WithMany(folder => folder.Files)
               .HasForeignKey(file => file.FolderId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
