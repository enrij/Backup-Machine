using BackupMachine.Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackupMachine.Infrastructure.Persistence.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("Jobs");

        builder.HasKey(job => job.Id);

        builder.HasData(new List<Job>
        {
            new() { Id = Guid.Parse("e005279a-2b23-4a3c-b798-27cb443daf9e"), Name = "Test", Source = @"C:\Temp\Sources\SmallSource", Destination = @"C:\Temp\Backups\SmallSource" }
        });
    }
}
