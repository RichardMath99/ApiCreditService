using FlagshipAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FlagshipAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CreditParameter> CreditParameters { get; set; }
    }
}
