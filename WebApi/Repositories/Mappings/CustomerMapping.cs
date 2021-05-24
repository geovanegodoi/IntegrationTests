using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Models;

namespace WebApi.Repositories.Mappings
{
    public class CustomerMapping : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.ConfigureBaseEntity();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(c => c.Email)
                .IsRequired()
                .HasColumnType("varchar(100)");
        }
    }

    public static class EntityFrameworkExtensions
    {
        public static void ConfigureBaseEntity<T>(this EntityTypeBuilder<T> builder) where T : class
        {
            if (typeof(T).IsAssignableFrom(typeof(BaseEntity)))
            {
                var builderBase = (builder as EntityTypeBuilder<BaseEntity>);
                builderBase.HasKey(p => p.Id);
            }
        }
    }
}
