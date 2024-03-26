using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Data
{
    public class ServerDbContext: DbContext
    {
        public ServerDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
