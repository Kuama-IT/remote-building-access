using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLocks.Repositories;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.NukiActions.Repositories;
using Kerbero.Infrastructure.NukiCredentials.Repositories;
using Kerbero.Infrastructure.SmartLocks.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Kerbero.Infrastructure;

public static class ConfigureService
{
  public static void AddInfrastructureServices(this IServiceCollection services)
  {
    services.AddScoped<INukiCredentialRepository, NukiCredentialRepository>();
    services.AddScoped<INukiSmartLockExternalRepository, NukiSmartLockExternalRepository>();
    services.AddScoped<INukiSmartLockPersistentRepository, NukiSmartLockPersistentRepository>();
    services.AddScoped<INukiSmartLockRepository, NukiSmartLockRepository>();

    services.AddScoped<NukiSafeHttpCallHelper>();
  }
}
