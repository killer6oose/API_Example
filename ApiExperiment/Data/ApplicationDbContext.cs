using ApiExperiment.Models;
using Microsoft.EntityFrameworkCore;

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
