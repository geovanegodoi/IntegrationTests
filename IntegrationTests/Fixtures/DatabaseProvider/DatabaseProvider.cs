using System;
using static WebApi.StartupTest;

namespace IntegrationTests
{
    public class DatabaseProvider : IDisposable
    {
        private IDatabaseInstance _dbInstance = null;

        public DatabaseProvider UseLocalhost()
        {
            _dbInstance = new LocalhostDatabaseInstance();
            return this;
        }

        public DatabaseProvider UseDocker()
        {
            _dbInstance = new DockerDatabaseInstance();
            return this;
        }

        public DatabaseProvider UseSqlServer()
        {
            InitializeDatabase(DbProviderType.MSSQL);
            return this;
        }

        public DatabaseProvider UseOracle()
        {
            InitializeDatabase(DbProviderType.ORACLE);
            return this;
        }

        public DatabaseProvider UsePostgres()
        {
            InitializeDatabase(DbProviderType.POSTGRES);
            return this;
        }

        public string GetConnectionString()
        {
            AssertInstanceNotNull();
            return _dbInstance.GetConnectionString();
        }

        private void InitializeDatabase(DbProviderType providerType)
        {
            AssertInstanceNotNull();
            _dbInstance.SetupProvider(providerType);
            _dbInstance.Initialize();
        }

        private void AssertInstanceNotNull()
        {
            if (_dbInstance == null)
                throw new NullReferenceException("Must setup Database Instance before access it");
        }

        public void Dispose()
            =>  _dbInstance?.Dispose();
    }
}
