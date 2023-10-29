using API.FumitureStore.Data;
using API.FumitureStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace API.FumitureStore.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsCategoriesController : ControllerBase
    {
        private readonly APIFumitureStoreContext _context;

        public ProductsCategoriesController(APIFumitureStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductCategory>> Get()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetDetails(int Id)
        {
            var productCategory = await _context.ProductCategories.FirstOrDefaultAsync(prodCateg => prodCateg.Id == Id);
            if (productCategory == null)
            {
                return NotFound();
            }

            return Ok(productCategory);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductCategory productCategory)
        {
            await _context.ProductCategories.AddAsync(productCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Post", productCategory.Id, productCategory);

        }

        [HttpPut]
        public async Task<IActionResult> Put(ProductCategory productCategory)
        {
            _context.ProductCategories.Update(productCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(ProductCategory productCategory)
        {
            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
