using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using WebApi;
using WebApi.Repositories;

namespace IntegrationTests
{
    public class IntegrationTestsFixture : IDisposable
    {
        public WebAppFactory<StartupTest> Factory { get; protected set; }
        public HttpClient Client { get; protected set; }
        public DatabaseContext DbContext { get; protected set; }

        private DatabaseProvider DbProvider;

        public IntegrationTestsFixture()
        {
            DbProvider = new DatabaseProvider().UseLocalhost().UseSqlServer();
            var connectionString = DbProvider.GetConnectionString();
            CreateWebHostApplication(connectionString);
            CreateDatabaseContext(connectionString);
        }

        protected void CreateWebHostApplication(string connectionString)
        {
            StartupTest.ConnectionString = connectionString;
            var clientOptions = new WebApplicationFactoryClientOptions();
            Factory = new WebAppFactory<StartupTest>();
            Client = Factory.CreateClient(clientOptions);
        }

        protected void CreateDatabaseContext(string connectionString)
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>().UseSqlServer(connectionString, providerOptions => providerOptions.EnableRetryOnFailure());
            DbContext   = new DatabaseContext(builder.Options);
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbProvider.Dispose();
            Client.Dispose();
            Factory.Dispose();
        }
    }
}
