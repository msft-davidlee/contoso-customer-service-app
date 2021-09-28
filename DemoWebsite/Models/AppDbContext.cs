using Microsoft.EntityFrameworkCore;

namespace DemoWebsite.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<RewardCustomer> RewardCustomers { get; set; }
        public DbSet<RewardItem> RewardItems { get; set; }
    }
}
