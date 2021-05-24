using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using WebApi.Repositories;

namespace WebApi
{
    public class StartupTest
    {
        public enum DbProviderType { MSSQL, ORACLE, POSTGRES };

        public static DbProviderType ProviderType { get; set; }
        public static string ConnectionString { get; set; }

        public StartupTest(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(GetDatabaseConfiguration());

            services.AddControllers();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }

        private Action<DbContextOptionsBuilder> GetDatabaseConfiguration()
            => ProviderType switch
            {
                DbProviderType.MSSQL    => UseSqlServerDatabase(),
                DbProviderType.ORACLE   => UseOracleDatabase(),
                DbProviderType.POSTGRES => UsePostgresDatabase(),
                _ => throw new ArgumentOutOfRangeException(nameof(ProviderType), $"Not expected value: {ProviderType}")
            };

        private Action<DbContextOptionsBuilder> UseSqlServerDatabase()
            => options => options.UseSqlServer(ConnectionString, providerOptions => providerOptions.EnableRetryOnFailure());

        private Action<DbContextOptionsBuilder> UseOracleDatabase()
            => options => options.UseOracle(ConnectionString);

        private Action<DbContextOptionsBuilder> UsePostgresDatabase()
            => options => options.UseNpgsql(ConnectionString, providerOptions => providerOptions.EnableRetryOnFailure());

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
