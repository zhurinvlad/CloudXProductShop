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
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }

}
