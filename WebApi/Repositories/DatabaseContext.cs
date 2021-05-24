using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) : base(dbContextOptions)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupDefautTypes(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
            
            base.OnModelCreating(modelBuilder);
        }

        private void SetupDefautTypes(ModelBuilder modelBuilder)
        {
            var stringProperties = modelBuilder.Model.GetEntityTypes()
                                    .SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string)));

            foreach (var property in stringProperties)
                property.SetColumnType("varchar(100)");
        }
    }
}
