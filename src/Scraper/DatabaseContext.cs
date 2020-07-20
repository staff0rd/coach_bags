using Microsoft.EntityFrameworkCore;

namespace coach_bags_selenium.Data
{
    public class DatabaseContext : DbContext
    {
        private readonly string _connectionString;

        public DatabaseContext()
        {
            _connectionString = "User ID=postgres;Password=example;Host=localhost;Database=coach_bags;";
        }

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(_connectionString);
    }
}