using System;
using static WebApi.StartupTest;

namespace IntegrationTests
{
    public interface IDatabaseInstance : IDisposable
    {
        void Initialize();
        string GetConnectionString();
        void SetupProvider(DbProviderType dbProviderType);
    }
}
