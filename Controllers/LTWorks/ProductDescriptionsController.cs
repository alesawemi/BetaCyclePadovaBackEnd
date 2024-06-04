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
    public class ProductDescriptionsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public ProductDescriptionsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all product descriptions.
        /// </summary>
        /// <returns>A list of product descriptions.</returns>
        // GET: api/ProductDescriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDescription>>> GetProductDescriptions()
        {
            return await _context.ProductDescriptions.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific product description by ID.
        /// </summary>
        /// <param name="id">The ID of the product description.</param>
        /// <returns>The product description with the specified ID.</returns>
        // GET: api/ProductDescriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDescription>> GetProductDescription(int id)
        {
            var productDescription = await _context.ProductDescriptions.FindAsync(id);

            if (productDescription == null)
            {
                return NotFound();
            }

            return productDescription;
        }

        /// <summary>
        /// Updates a specific product description by ID.
        /// </summary>
        /// <param name="id">The ID of the product description to update.</param>
        /// <param name="productDescription">The updated product description object.</param>
        /// <returns>No content if the update is successful.</returns>
        // PUT: api/ProductDescriptions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductDescription(int id, ProductDescription productDescription)
        {
            if (id != productDescription.ProductDescriptionId)
            {
                return BadRequest();
            }

            _context.Entry(productDescription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductDescriptionExists(id))
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
        /// Adds a new product description.
        /// </summary>
        /// <param name="productDescription">The product description to add.</param>
        /// <returns>The newly created product description.</returns>
        // POST: api/ProductDescriptions
        [HttpPost]
        public async Task<ActionResult<ProductDescription>> PostProductDescription(ProductDescription productDescription)
        {
            _context.ProductDescriptions.Add(productDescription);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductDescription", new { id = productDescription.ProductDescriptionId }, productDescription);
        }

        /// <summary>
        /// Deletes a specific product description by ID.
        /// </summary>
        /// <param name="id">The ID of the product description to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        // DELETE: api/ProductDescriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductDescription(int id)
        {
            var productDescription = await _context.ProductDescriptions.FindAsync(id);
            if (productDescription == null)
            {
                return NotFound();
            }

            _context.ProductDescriptions.Remove(productDescription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a product description with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the product description.</param>
        /// <returns>True if the product description exists, otherwise false.</returns>
        private bool ProductDescriptionExists(int id)
        {
            return _context.ProductDescriptions.Any(e => e.ProductDescriptionId == id);
        }
    }
}
