using Docker.DotNet.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class SqlServerContainerProvider : IDatabaseContainerProvider
    {
        // docker run --name MSSQLSERVER -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=12345678" -p 1401:1433  -d mcr.microsoft.com/mssql/server:2019-latest

        private const string CONTAINER_NAME_PREFIX = "MSSQLSERVER";
        private const string CONTAINER_IMAGE       = "mcr.microsoft.com/mssql/server:2019-latest";
        private const string DATABASE_SA_PASSWORD  = "P@ssword123!";
        private const string DATABASE_NAME         = "dockerdb";
        private const string DATABASE_PORT         = "1401";

        public string GetContainerName() => CONTAINER_NAME_PREFIX;
        
        public string GetImageName() => CONTAINER_IMAGE;

        public IList<string> GetEnvironmentVariables()
            => new List<string>
            {
                "ACCEPT_EULA=Y",
                $"SA_PASSWORD={DATABASE_SA_PASSWORD}"
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
        {
            return $"Server=localhost,{DATABASE_PORT};" +
                   $"Database={DATABASE_NAME};" +
                   $"User ID=sa;" +
                   $"Password={DATABASE_SA_PASSWORD}";
        }

        public bool TryDatabaseConnection()
        {
            try
            {
                using var sqlConnection = new SqlConnection(GetConnectionString());
                sqlConnection.OpenAsync().GetAwaiter();
                return true;
            }
            catch
            {
                // If opening the SQL connection fails, SQL Server is not ready yet
                Task.Delay(500);
                return false;
            }
        }
    }
}