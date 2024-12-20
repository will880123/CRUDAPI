using CRUDAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CRUDAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
