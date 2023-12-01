using Microsoft.EntityFrameworkCore;
using SamWarehouse.Models;

namespace SamWarehouse.Models
    {
    public class ItemDbContext : DbContext
        {
        public ItemDbContext() { }

        public ItemDbContext(DbContextOptions options) : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Comment> Comments { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
            if (!optionsBuilder.IsConfigured)
                { }
            }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {

            /* modelBuilder.Entity<Author>().HasData(
               new Author { Id = 1, FirstName = "Dougle", LastName = "Dickson" },
               new Author { Id = 2, FirstName = "Tom", LastName = "Clancy" },
               new Author { Id = 3, FirstName = "Margaret", LastName = "Atwood" }
               );

             modelBuilder.Entity<Book>().HasData(
                 new Book { Id = 1, Title = "Man After Man", Year = 2004, AuthorId = 1, Price = 7.99 },
                 new Book { Id = 2, Title = "Rainbow Six", Year = 1998, AuthorId = 2, Price = 12.99 },
                 new Book { Id = 3, Title = "Airforce One", Year = 1988, AuthorId = 2, Price = 10.99 },
                 new Book { Id = 4, Title = "A Handmaids Tale", Year = 2010, AuthorId = 3, Price = 10.99 }
                 );*/

            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                    {
                    UserId = 1,
                    UserName = "Anu",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Password_1"),
                    Role = "Admin"
                    },
                  new AppUser
                      {
                      UserId = 2,
                      UserName = "Troy",
                      Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Password_1"),
                      Role = "Admin"
                      }
                  );
            modelBuilder.Entity<Product>().HasData(
                new Product
                    {
                    ProductCode = 1,
                    ProductName = "Anu",
                    ProductDescription = "Test 24",
                    ProductPrice = 20.23,
                    UpdatedDate = DateTime.Now,
                    },
                  new Product
                      {
                      ProductCode = 2,
                      ProductName = "Shirt",
                      ProductDescription = "Test tyty 24",
                      ProductPrice = 26.23,
                      UpdatedDate = DateTime.Now,
                      }
                  );

            }
        }
    }
