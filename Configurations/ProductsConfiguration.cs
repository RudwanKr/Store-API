using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store_API.Models;

namespace Store_API.Configurations
{
    public class ProductsConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.ID);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);

            builder.Property(p => p.Price).IsRequired();
            builder.Property(p => p.Quantity).IsRequired();
        }
    }
}
