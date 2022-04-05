using BackupMachine.PoC.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackupMachine.PoC.Infrastructure.Persistence.Configurations;

public class BackupFolderConfiguration : IEntityTypeConfiguration<BackupFolder>
{
    public void Configure(EntityTypeBuilder<BackupFolder> builder)
    {
        builder.ToTable("Folders");

        builder.HasKey(folder => folder.Id);

        builder.HasOne(folder => folder.Backup)
               .WithMany()
               .HasForeignKey(folder => folder.BackupId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(folder => folder.ParentFolder)
               .WithMany(parent => parent.Subfolders)
               .HasForeignKey(folder => folder.ParentFolderId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
