using BackupMachine.PoC.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackupMachine.PoC.Infrastructure.Persistence.Configurations;

public class FolderConfiguration : IEntityTypeConfiguration<FolderCopy>
{
    public void Configure(EntityTypeBuilder<FolderCopy> builder)
    {
        builder.ToTable("Folders");

        builder.HasKey(folder => folder.Id);

        builder.Property(folder => folder.Id)
               .ValueGeneratedOnAdd();

        builder.HasOne(folder => folder.PreviousVersion)
               .WithOne()
               .HasForeignKey<FolderCopy>(previous => previous.PreviousVersionId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(folder => folder.Snapshot)
               .WithMany()
               .HasForeignKey(folder => folder.SnapshotId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(folder => folder.ParentFolder)
               .WithMany(parent => parent.Folders)
               .HasForeignKey(folder => folder.ParentFolderId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
