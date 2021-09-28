using Microsoft.EntityFrameworkCore;

namespace DemoCustomerServiceAltId.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<AlternateId> AlternateIds { get; set; }        
    }
}
