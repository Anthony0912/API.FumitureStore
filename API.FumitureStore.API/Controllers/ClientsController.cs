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
    public class ClientsController : ControllerBase
    {
        private readonly APIFumitureStoreContext _context;

        public ClientsController(APIFumitureStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Client>> Get()
        {
            return await _context.Clients.ToListAsync();
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetDetails(int Id)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(client => client.Id == Id);
            if (client == null) {
                return NotFound();
            }

            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Client client)
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction("Post", client.Id, client);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Client client)
        {
            if (client == null) 
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
