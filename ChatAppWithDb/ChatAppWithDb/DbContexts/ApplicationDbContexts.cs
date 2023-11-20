using ChatAppWithDb.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppWithDb.DbContexts
{
    public class ApplicationDbContexts : DbContext
    {
        public ApplicationDbContexts(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
