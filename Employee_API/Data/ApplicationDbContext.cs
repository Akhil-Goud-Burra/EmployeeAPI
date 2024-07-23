using Employee_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Employee_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employee_Table { get; set; }
    }
}
