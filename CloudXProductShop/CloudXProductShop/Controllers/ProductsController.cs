using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.XRay.Recorder.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudXProductShop.DAL;
using Microsoft.VisualBasic;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CloudXProductShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductShopContext _context;
        private readonly ILogger<ProductsController> _logger;
        private readonly string CACHE_KEY = "list-products";
        public ProductsController(ProductShopContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            //var db = getCacheDb();
            //var cacheProducts = db.StringGet(CACHE_KEY);

            IEnumerable<Product> products = null;

            //if (cacheProducts.IsNullOrEmpty)
            //{
                products = await _context.Products.ToListAsync();
            //    db.StringSet(CACHE_KEY, JsonSerializer.Serialize(products));
            //}
            //else
            //{
                //products = JsonSerializer.Deserialize<IEnumerable<Product>>(cacheProducts);
            //}
            return Ok(products);
        }

        // GET: api/Products
        [HttpGet("test")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsTest()
        {
            AWSXRayRecorder.Instance.TraceMethod("Test xRay", () => _logger.LogInformation("It works!"));
            return Ok( new
            {
                Message="It works!"
            });
        }

        // GET: api/Products
        [HttpGet("test2")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsTest2()
        {
            _logger.LogInformation("It works 2!");
            return Ok(new
            {
                Message = "It works 2!"
            });
        }
        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var db = getCacheDb();
            db.KeyDelete(CACHE_KEY);
            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        private IDatabase getCacheDb()
        {
            var cacheEndPoint = "localhost:6379"; // TODO
            var redis = ConnectionMultiplexer.Connect(cacheEndPoint);
            return redis.GetDatabase();
        }
    }
}
