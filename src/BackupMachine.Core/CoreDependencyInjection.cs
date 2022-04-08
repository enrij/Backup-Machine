using BackupMachine.Core.Services;

using Microsoft.Extensions.DependencyInjection;

namespace BackupMachine.Core;

public static class CoreDependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        return services.AddSingleton<BackupsService>()
                       .AddSingleton<JobsService>();
    }
}
