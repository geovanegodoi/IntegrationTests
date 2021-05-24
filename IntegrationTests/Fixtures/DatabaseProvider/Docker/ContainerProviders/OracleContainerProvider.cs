using Docker.DotNet.Models;
using System.Collections.Generic;

namespace IntegrationTests
{
    public class OracleContainerProvider : IDatabaseContainerProvider
    {
        // docker run -d -it --name ORACLEDB store/oracle/database-enterprise:12.2.0.1

        private const string CONTAINER_NAME_PREFIX = "ORACLE";
        private const string CONTAINER_IMAGE       = "store/oracle/database-enterprise:12.2.0.1";
        private const string DATABASE_SA_PASSWORD  = "P@ssword123!";
        private const string DATABASE_NAME         = "DOCKERDB";
        private const string DATABASE_PORT         = "1401";

        public string GetContainerName() => CONTAINER_NAME_PREFIX;
        
        public string GetImageName() => CONTAINER_IMAGE;

        public IList<string> GetEnvironmentVariables()
            => new List<string>();

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
               $"Password={DATABASE_SA_PASSWORD}";

        public bool TryDatabaseConnection()
        {
            throw new System.NotImplementedException();
        }
    }
}