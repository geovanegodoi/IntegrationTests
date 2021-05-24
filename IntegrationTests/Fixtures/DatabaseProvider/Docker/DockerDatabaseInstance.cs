using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApi;
using static WebApi.StartupTest;

namespace IntegrationTests
{
    public class DockerDatabaseInstance : IDatabaseInstance
    {
        private DockerClient _dockerClient;
        private IDatabaseContainerProvider _containerProvider;
        private CreateContainerResponse _container;

        public DockerDatabaseInstance()
        {
            _dockerClient = CreateDockerClient();
        }

        public void Initialize()
        {
            AssertProviderIsSetup();

            CleanupRunningContainers();

            CreateContainer();

            _dockerClient.Containers.StartContainerAsync(_container.ID, new ContainerStartParameters()).GetAwaiter();

            WaitUntilDatabaseAvailable();
        }

        public string GetConnectionString()
            => _containerProvider.GetConnectionString();

        public void Dispose()
        {
            EnsureDockerStoppedAndRemovedAsync(_container.ID);
            _dockerClient.Dispose();
        }

        private void AssertProviderIsSetup()
        {
            if (_containerProvider == null)
                throw new NullReferenceException("You must setup DbProvider before first use");
        }

        private void EnsureDockerStoppedAndRemovedAsync(string dockerContainerId)
        {
            _dockerClient.Containers.StopContainerAsync(dockerContainerId, new ContainerStopParameters()).GetAwaiter();
            _dockerClient.Containers.RemoveContainerAsync(dockerContainerId, new ContainerRemoveParameters()).GetAwaiter();
        }

        private void CleanupRunningContainers()
        {
            var runningContainers = _dockerClient.Containers.ListContainersAsync(new ContainersListParameters()).Result;

            foreach (var runningContainer in runningContainers.Where(MatchContainerName()))
            {
                try
                {
                    EnsureDockerStoppedAndRemovedAsync(runningContainer.ID);
                }
                catch
                {
                    // Ignoring failures to stop running containers
                }
            }
        }

        private void CreateContainer()
        {
            _dockerClient.Images.CreateImageAsync(GetImagesCreateParameters(), null, new Progress<JSONMessage>()).GetAwaiter();

            _container = _dockerClient.Containers.CreateContainerAsync(GetCreateContainerParameters()).Result;
        }

        private Func<ContainerListResponse, bool> MatchContainerName()
            => (container) => container.Names.Any(n => n.Contains(_containerProvider.GetContainerName()));

        private bool IsRunningMoreThanOneHour(ContainerListResponse container)
            => container.Created < DateTime.UtcNow.AddHours(-1);

        private ImagesCreateParameters GetImagesCreateParameters()
            => new ImagesCreateParameters
            {
                FromImage = _containerProvider.GetImageName()
            };

        private CreateContainerParameters GetCreateContainerParameters()
            => new CreateContainerParameters
            {

                Name = _containerProvider.GetContainerName() + "-" + Guid.NewGuid(),
                Image = _containerProvider.GetImageName(),
                Env = _containerProvider.GetEnvironmentVariables(),
                HostConfig = GetHostConfig()
            };

        private HostConfig GetHostConfig()
            => new HostConfig()
            {
                PortBindings = _containerProvider.GetPortBindings()
            };

        private DockerClient CreateDockerClient()
        {
            var dockerUri = IsRunningOnWindows()
                ? "npipe://./pipe/docker_engine"
                : "unix:///var/run/docker.sock";

            return new DockerClientConfiguration(new Uri(dockerUri)).CreateClient();
        }

        private bool IsRunningOnWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        private void WaitUntilDatabaseAvailable()
        {
            var start = DateTime.UtcNow;
            const int maxWaitTimeSeconds = 60;
            var connectionEstablished = false;
            while (!connectionEstablished && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
            {
                connectionEstablished = _containerProvider.TryDatabaseConnection();
            }
            if (!connectionEstablished)
            {
                throw new Exception("Connection to the SQL docker database could not be established within 60 seconds.");
            }
        }

        public void SetupProvider(DbProviderType dbProviderType)
        {
            _containerProvider = dbProviderType switch
            {
                DbProviderType.MSSQL    => new SqlServerContainerProvider(),
                DbProviderType.ORACLE   => new OracleContainerProvider(),
                DbProviderType.POSTGRES => new PostgresContainerProvider(),
                _ => throw new ArgumentOutOfRangeException(nameof(dbProviderType), $"Not expected value: {dbProviderType}")
            };
        }
    }
}
