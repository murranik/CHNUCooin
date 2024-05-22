using CHNUCooin.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CHNUCooin.Database
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        
        public DbSet<User> Users { get; set; }
        
        public DbSet<UserPrivate> UserPrivates { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

    }
}
