using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using server.Data;
using server.Models;

namespace server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly ServerDbContext _serverDbContext;
        public ProductsController(ServerDbContext serverDbContext)
        {
            this._serverDbContext = serverDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _serverDbContext.Products.ToListAsync();
            return Ok(products);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            product.Id = Guid.NewGuid();
            await _serverDbContext.Products.AddAsync(product);
            await _serverDbContext.SaveChangesAsync();
            return Ok(product);
        }

        [Authorize]
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = await _serverDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [Authorize]
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, Product updateProductRequest)
        {
            var product = await _serverDbContext.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            product.Name = updateProductRequest.Name;
            product.Type = updateProductRequest.Type;
            product.Price = updateProductRequest.Price;
            product.Color = updateProductRequest.Color;

            await _serverDbContext.SaveChangesAsync();

            return Ok(product);
        }

        [Authorize]
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {

            var product = await _serverDbContext.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            _serverDbContext.Products.Remove(product);
            await _serverDbContext.SaveChangesAsync();

            return Ok(product);
        }

        [HttpGet("sorted")]
        public async Task<IEnumerable<Product>> GetProducts(string sortBy, string sortOrder)
        {
            IQueryable<Product> productsQuery = _serverDbContext.Products;

            // Sort products based on sortBy and sortOrder
            if (sortBy == "price")
            {
                if (sortOrder == "asc")
                {
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                }
                else if (sortOrder == "desc")
                {
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                }
            }

            // Execute the query asynchronously and return the results
            return await productsQuery.ToListAsync();
        }
    }
}
