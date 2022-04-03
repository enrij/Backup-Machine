using BackupMachine.PoC.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackupMachine.PoC.Infrastructure.Persistence.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("Jobs");

        builder.HasKey(job => job.Id);

        builder.HasData(new List<Job>
        {
            new() { Id = Guid.NewGuid(), Name = "Test", Source = @"C:\Temp\Sources\SmallSource", Destination = @"C:\Temp\Backups\SmallSource" }
        });
    }
}
