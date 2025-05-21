using System.Data.Entity;
using RestaurantManagementApp.DataAccess.Configuration;
using RestaurantManagementApp.Models;

namespace RestaurantManagementApp.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base(ConnectionStringProvider.GetConnectionString())
        {
           
            Database.SetInitializer(new DatabaseInitializer());
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuProduct> MenuProducts { get; set; }
        public DbSet<Allergen> Allergens { get; set; }
        public DbSet<ProductAllergen> ProductAllergens { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<MenuProduct>()
                .HasKey(mp => new { mp.MenuId, mp.ProductId });

            modelBuilder.Entity<ProductAllergen>()
                .HasKey(pa => new { pa.ProductId, pa.AllergenId });

            
            modelBuilder.Entity<MenuProduct>()
                .HasRequired(mp => mp.Menu)
                .WithMany(m => m.MenuProducts)
                .HasForeignKey(mp => mp.MenuId);

            modelBuilder.Entity<MenuProduct>()
                .HasRequired(mp => mp.Product)
                .WithMany(p => p.MenuProducts)
                .HasForeignKey(mp => mp.ProductId);

            modelBuilder.Entity<ProductAllergen>()
                .HasRequired(pa => pa.Product)
                .WithMany(p => p.ProductAllergens)
                .HasForeignKey(pa => pa.ProductId);

            modelBuilder.Entity<ProductAllergen>()
                .HasRequired(pa => pa.Allergen)
                .WithMany(a => a.ProductAllergens)
                .HasForeignKey(pa => pa.AllergenId);

            
            modelBuilder.Entity<Product>()
                .HasRequired(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Menu>()
                .HasRequired(m => m.Category)
                .WithMany(c => c.Menus)
                .HasForeignKey(m => m.CategoryId);

            modelBuilder.Entity<Order>()
                .HasRequired(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

          
            base.OnModelCreating(modelBuilder);
        }
    }
}