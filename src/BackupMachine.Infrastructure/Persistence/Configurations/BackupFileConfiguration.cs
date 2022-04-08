using BackupMachine.Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackupMachine.Infrastructure.Persistence.Configurations;

public class BackupFileConfiguration : IEntityTypeConfiguration<BackupFile>
{
    public void Configure(EntityTypeBuilder<BackupFile> builder)
    {
        builder.ToTable("Files");

        builder.HasKey(file => file.Id);

        builder.HasOne(file => file.Backup)
               .WithMany()
               .HasForeignKey(file => file.BackupId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(file => file.BackupFolder)
               .WithMany(folder => folder.Files)
               .HasForeignKey(file => file.BackupFolderId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
