using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace BackupMachine.Core;

public static class CoreDependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        return services.AddMediatR(configuration => { configuration.AsScoped(); }, typeof(CoreAssemblyMarker));
    }
}
