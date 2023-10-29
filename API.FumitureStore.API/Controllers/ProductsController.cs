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
    public class ProductsController : ControllerBase
    {
        private readonly APIFumitureStoreContext _context;

        public ProductsController(APIFumitureStoreContext context)
        {
            this._context = context;
        }
        //Para omitir si un metodo sea publico [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetDetails(int Id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(product => product.Id == Id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet("GetByCategory/{productCategoryId}")]
        public async Task<IEnumerable<Product>> GetByCategory(int productCategoryId)
        {
            return await _context.Products
                .Where(product => product.ProductCategoryId == productCategoryId)
                .ToArrayAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post(Product product) { 
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction("Post", product.Id, product);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
