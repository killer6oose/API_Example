using Microsoft.EntityFrameworkCore;
using ApiExperiment.Models;

namespace ApiExperiment.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserData> UserData { get; set; }
    }
}
