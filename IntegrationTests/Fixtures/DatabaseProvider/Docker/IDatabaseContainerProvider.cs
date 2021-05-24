using Docker.DotNet.Models;
using System.Collections.Generic;

namespace IntegrationTests
{
    public interface IDatabaseContainerProvider
    {
        string GetContainerName();
        string GetImageName();
        IList<string> GetEnvironmentVariables();
        Dictionary<string, IList<PortBinding>> GetPortBindings();
        string GetConnectionString();
        bool TryDatabaseConnection();
    }
}