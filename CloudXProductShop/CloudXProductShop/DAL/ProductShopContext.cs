using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CloudXProductShop.DAL
{
    public class ProductShopContext : DbContext
    {
        public ProductShopContext(DbContextOptions<ProductShopContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartProduct>()
                .HasKey(t => new { t.CartId, t.ProductId });

            modelBuilder.Entity<CartProduct>()
                .HasOne(pt => pt.Cart)
                .WithMany(p => p.CartProducts)
                .HasForeignKey(pt => pt.CartId);

            modelBuilder.Entity<CartProduct>()
                .HasOne(pt => pt.Product)
                .WithMany(t => t.CartProducts)
                .HasForeignKey(pt => pt.ProductId);
        }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public List<CartProduct> CartProducts { get; set; }
    }

    public class Cart
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public List<CartProduct> CartProducts { get; set; }
    }
    public class Order
    {
        public string Id { get; set; }
        public int CartId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
    public class CartProduct
    {
        public int Count { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
