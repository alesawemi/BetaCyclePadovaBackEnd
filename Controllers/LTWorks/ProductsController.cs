using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Models.LTWorks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using BetaCycle_Padova.BLogic.Authentication.Basic;
using NLog;

namespace BetaCycle_Padova.Controllers.LTWorks
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public ProductsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        private static Logger ProductsNlogLogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A list of products.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        /// <summary>
        /// Retrieves products by pages.
        /// </summary>
        /// <returns>A list of products paginated.</returns>
        [Route("GetProductsByPage")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByPage()
        {
            int lastId = 0;
            int rowsPage = 12;

            var page = _context.Products.FromSql(
                $"SELECT * FROM [AdventureWorksLT2019].[SalesLT].[Product]")
                .OrderBy(ob => ob.Name)
                .ThenBy(ob => ob.StandardCost)
                .Where(a => a.ProductId > lastId)
                .Take(rowsPage)
                .ToListAsync();

            return await page;
        }

        /// <summary>
        /// Retrieves a specific product by ID.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The product with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            ProductsNlogLogger.Info("Products Controller - Get by Id");
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                ProductsNlogLogger.Info("Products Controller - Get by Id - Not Found");
                return NotFound();
            }

            return product;
        }

        /// <summary>
        /// Updates a specific product by ID.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="product">The updated product object.</param>
        /// <returns>No content if the update is successful.</returns>
        /// 
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                ProductsNlogLogger.Info("Products Controller - Put(id, product) - BadRequest: id != product.id");
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                ProductsNlogLogger.Info("Products Controller - Put");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProductExists(id))
                {
                    ProductsNlogLogger.Error(ex, "Products Controller - Put - The id does not exist in the DB");
                    return NotFound();
                }
                else
                {
                    ProductsNlogLogger.Error(ex, "Products Controller - Put - A DbUpdateConcurrencyException occurred");
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Adds a new product.
        /// </summary>
        /// <param name="product">The product to add.</param>
        /// <returns>The newly created product.</returns>
        /// [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            ProductsNlogLogger.Info("Products Controller - Post");

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        /// <summary>
        /// Deletes a specific product by ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            ProductsNlogLogger.Info("Products Controller - Delete");

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                ProductsNlogLogger.Info("Products Controller - Not Found");
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a product with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>True if the product exists, otherwise false.</returns>
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

    }
}
