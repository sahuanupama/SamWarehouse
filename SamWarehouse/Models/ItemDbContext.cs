using Microsoft.EntityFrameworkCore;
using SamWarehouse.Models;

namespace SamWarehouse.Models
    {
    public class ItemDbContext : DbContext
        {
        public ItemDbContext() { }

        public ItemDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
            if (!optionsBuilder.IsConfigured)
                { }
            }

        public DbSet<SamWarehouse.Models.ShoppingCart> ShoppingCart { get; set; } = default!;
        }
    }
