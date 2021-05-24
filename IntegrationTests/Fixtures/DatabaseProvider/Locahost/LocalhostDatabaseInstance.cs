using System;
using WebApi;
using static WebApi.StartupTest;

namespace IntegrationTests
{
    public class LocalhostDatabaseInstance : IDatabaseInstance
    {
        private DbProviderType CurrentDbProvider;

        public void Initialize()
        {}

        public string GetConnectionString()
            => CurrentDbProvider switch
            {
                DbProviderType.MSSQL    => "Server=localhost,1401;Database=dockerdb;User Id=sa;Password=P@ssword123!;",
                DbProviderType.ORACLE   => "Server=localhost,1401;Database=dockerdb;User Id=sa;Password=P@ssword123!;",
                DbProviderType.POSTGRES => "Server=localhost,1401;Database=dockerdb;User Id=sa;Password=P@ssword123!;",
                _ => throw new ArgumentOutOfRangeException(nameof(CurrentDbProvider), $"Not expected value: {CurrentDbProvider}")
            };

        public void SetupProvider(DbProviderType dbProviderType)
            => CurrentDbProvider = dbProviderType;

        public void Dispose()
        {}
    }
}
