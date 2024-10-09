using Microsoft.EntityFrameworkCore;
using KeycloakTesting.Models;

namespace KeycloakTesting.Data
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
