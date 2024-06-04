using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Models.LTWorks;

namespace BetaCycle_Padova.Controllers.LTWorks
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public ProductCategoriesController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all product categories.
        /// </summary>
        /// <returns>A list of product categories.</returns>
        // GET: api/ProductCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific product category by ID.
        /// </summary>
        /// <param name="id">The ID of the product category.</param>
        /// <returns>The product category with the specified ID.</returns>
        // GET: api/ProductCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> GetProductCategory(int id)
        {
            var productCategory = await _context.ProductCategories.FindAsync(id);

            if (productCategory == null)
            {
                return NotFound();
            }

            return productCategory;
        }

        /// <summary>
        /// Updates a specific product category by ID.
        /// </summary>
        /// <param name="id">The ID of the product category to update.</param>
        /// <param name="productCategory">The updated product category object.</param>
        /// <returns>No content if the update is successful.</returns>
        // PUT: api/ProductCategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductCategory(int id, ProductCategory productCategory)
        {
            if (id != productCategory.ProductCategoryId)
            {
                return BadRequest();
            }

            _context.Entry(productCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductCategoryExists(id))
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

        /// <summary>
        /// Adds a new product category.
        /// </summary>
        /// <param name="productCategory">The product category to add.</param>
        /// <returns>The newly created product category.</returns>
        // POST: api/ProductCategories
        [HttpPost]
        public async Task<ActionResult<ProductCategory>> PostProductCategory(ProductCategory productCategory)
        {
            _context.ProductCategories.Add(productCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductCategory", new { id = productCategory.ProductCategoryId }, productCategory);
        }

        /// <summary>
        /// Deletes a specific product category by ID.
        /// </summary>
        /// <param name="id">The ID of the product category to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        // DELETE: api/ProductCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            var productCategory = await _context.ProductCategories.FindAsync(id);
            if (productCategory == null)
            {
                return NotFound();
            }

            _context.ProductCategories.Remove(productCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a product category with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the product category.</param>
        /// <returns>True if the product category exists, otherwise false.</returns>
        private bool ProductCategoryExists(int id)
        {
            return _context.ProductCategories.Any(e => e.ProductCategoryId == id);
        }
    }
}
