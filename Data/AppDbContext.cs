using Microsoft.EntityFrameworkCore;
using WebApp_Anti.Models;

namespace WebApp_Anti.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<SerpProductInstance> SerpProductInstances { get; set; }
        public DbSet<DiscardApproval> DiscardApprovals { get; set; }
        public DbSet<DiscardApprovalInput> DiscardApprovalInputs { get; set; }
    }
}
