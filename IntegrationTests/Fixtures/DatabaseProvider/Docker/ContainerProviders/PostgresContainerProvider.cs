using Docker.DotNet.Models;
using System.Collections.Generic;

namespace IntegrationTests
{
    public class PostgresContainerProvider : IDatabaseContainerProvider
    {
        // docker run --name POSTGRESDB -e POSTGRES_PASSWORD=P@ssword123! -p 1401:1433 -d postgres

        private const string CONTAINER_NAME_PREFIX = "POSTGRES";
        private const string CONTAINER_IMAGE       = "postgres";
        private const string DATABASE_PASSWORD     = "P@ssword123!";
        private const string DATABASE_NAME         = "DOCKERDB";
        private const string DATABASE_PORT         = "1401";

        public string GetContainerName() => CONTAINER_NAME_PREFIX;
        
        public string GetImageName() => CONTAINER_IMAGE;

        public IList<string> GetEnvironmentVariables()
            => new List<string>
            {
                $"POSTGRES_PASSWORD={DATABASE_PASSWORD}"
            };

        public Dictionary<string, IList<PortBinding>> GetPortBindings()
            => new Dictionary<string, IList<PortBinding>>
            {
                {
                    "1433/tcp",
                    new PortBinding[]
                    {
                        new PortBinding { HostPort = DATABASE_PORT }
                    }
                }
            };

        public string GetConnectionString()
            => $"Data Source=localhost,{DATABASE_PORT};" +
               $"Initial Catalog={DATABASE_NAME};" +
               $"Integrated Security=False;" +
               $"User ID=SA;" +
               $"Password={DATABASE_PASSWORD}";

        public bool TryDatabaseConnection()
        {
            throw new System.NotImplementedException();
        }
    }
}