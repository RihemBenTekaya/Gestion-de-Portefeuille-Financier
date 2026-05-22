using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DashboardData.Models;

namespace DashboardData.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets for each entity, every DbSet represents a table in the database.
        public DbSet<Asset> Assets { get; set; }
        public DbSet<PortfolioTransaction> PortfolioTransactions { get; set; }
        public DbSet<InvestmentBudget> InvestmentBudgets { get; set; }
    }
}
