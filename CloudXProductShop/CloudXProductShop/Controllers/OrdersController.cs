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
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace CloudXProductShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ProductShopContext _context;
        private readonly ILogger<OrdersController> _logger;
        private readonly IAmazonSQS _sqsClient;


        public OrdersController(ProductShopContext context, ILogger<OrdersController> logger, IAmazonSQS sqsClient)
        {
            _sqsClient = sqsClient;
            _context = context;
            _logger = logger;
        }


        [HttpPost("{cartId}")]
        public async Task<ActionResult<Product>> Post(string cartId)
        {
            var cart = await _context.Carts.Include(x => x.CartProducts).ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.Id == cartId);
            if (cart == default)
            {
                return NotFound();
            }

            string qUrl = "https://sqs.us-east-2.amazonaws.com/515001955773/Checkout.fifo";
            string messageBody  = JsonConvert.SerializeObject(
                    new
                    {
                        cartId,
                        Sum = cart.CartProducts?.Sum(x => x.Count * x.Product.Price) ?? 0,
                        MessageGroupId = 1,
                        MessageDeduplicationID = cartId
                    }
                    );
            var request = new SendMessageRequest
            {
                QueueUrl = qUrl,
                MessageGroupId = 1.ToString(),
                MessageDeduplicationId = Guid.NewGuid().ToString(),
                MessageBody = messageBody
            };
            SendMessageResponse responseSendMsg =
              await _sqsClient.SendMessageAsync(request);
            Console.WriteLine($"Message added to queue\n  {qUrl}");
            Console.WriteLine($"HttpStatusCode: {responseSendMsg.HttpStatusCode}");
            return Ok();
        }
    }
}
