using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Store_API.Models;
public class AppDBContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetails> OrderDetails { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dataSource = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = dataSource,
            InitialCatalog = "StoreDB2",
            IntegratedSecurity = true,
            TrustServerCertificate = true
        };

        optionsBuilder.UseSqlServer(builder.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}