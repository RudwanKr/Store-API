using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store_API.Models;

namespace Store_API.Configurations
{
    public class OrdersConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.ID);
            builder.Property(o => o.Date).IsRequired();
            builder.Property(o => o.TotalPrice).IsRequired();

            builder.HasOne(o => o.customer)
                   .WithMany(c => c.Orders)
                   .HasForeignKey(o => o.CustomerID);
        }
    }
}
