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
    public class AddToCart
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ProductShopContext _context;
        private readonly ILogger<CartsController> _logger;
        public CartsController(ProductShopContext context, ILogger<CartsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> Get(string id)
        {
            var cart = await _context.Carts.Include(x => x.CartProducts).FirstOrDefaultAsync(x => x.Id == id);
            if (cart == default)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] AddToCart model)
        {
            var cart = await _context.Carts.Include(x => x.CartProducts).FirstOrDefaultAsync(x => x.Id == id);
            if (cart == default)
            {
                return NotFound();
            }
            cart.CartProducts.Add(new CartProduct
            {
                ProductId = model.ProductId,
                Count = model.Count
            });
            await _context.SaveChangesAsync();
            return Ok(cart);
        }

        [HttpPost]
        public async Task<ActionResult<Cart>> Post(string id)
        {
            var cart = new Cart
            {
                Date = DateTime.Now,
                Id = Guid.NewGuid().ToString()
            };
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return Ok(cart);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Cart>> Delete(string id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
