using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BackupMachine.PoC.Infrastructure.Persistence.Converters;

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
