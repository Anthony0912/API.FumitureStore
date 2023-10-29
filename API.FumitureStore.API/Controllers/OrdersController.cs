using API.FumitureStore.Data;
using API.FumitureStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.FumitureStore.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly APIFumitureStoreContext _context;

        public OrdersController(APIFumitureStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
            return await _context.Orders.Include(o => o.OrderDetails).ToArrayAsync();
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetDetails(int Id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(order => order.Id == Id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Order order)
        {
            if (order == null)
            {
                return NotFound();
            }

            if (order.OrderDetails == null)
            {
                return BadRequest("Order should have at least one details");
            }

            await _context.Orders.AddAsync(order);

            //Guarda las detalles de las ordenes
            await _context.OrderDetails.AddRangeAsync(order.OrderDetails);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Post", order.Id, order);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Order order)
        {
            if (order == null)
            {
                return NotFound();
            }

            if (order.Id <= 0)
            {
                return NotFound();
            }
            var existingOrder = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == order.Id);

            if (existingOrder == null)
            {
                return NotFound();
            }
            existingOrder.OrderNumber = order.OrderNumber;
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.DeliveryDate = order.DeliveryDate;
            existingOrder.ClientId = order.ClientId;

            _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);

            _context.Orders.Update(existingOrder);
            await _context.AddRangeAsync(order.OrderDetails);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Order order)
        {
            if (order == null)
            {
                return NotFound();
            }

            var existingOrder = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == order.Id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);
            _context.Orders.Remove(existingOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
