using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BackupMachine.Infrastructure.Persistence.Converters;

public class DirectoryInfoConverter : ValueConverter<DirectoryInfo, string>
{
    public DirectoryInfoConverter() :
        base(
            toDb => toDb.FullName,
            fromDb => new DirectoryInfo(fromDb)
        )
    {
    }
}
