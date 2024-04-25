using Microsoft.EntityFrameworkCore;
using WebProjects.Models;

namespace WebProjects.Context
{
    public class AppDbContext:DbContext 
    {
        public AppDbContext(DbContextOptions options) :base(options)
        {
            
        }
        public DbSet<Product>Products { get; set; }
    }
}
