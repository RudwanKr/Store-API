using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store_API.Models;

namespace Store_API.Configurations
{
    public class CustomersConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.ID);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Phone).IsRequired().HasMaxLength(20);

            builder.HasMany(c => c.Orders)
                   .WithOne(o => o.customer)
                   .HasForeignKey(o => o.CustomerID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
