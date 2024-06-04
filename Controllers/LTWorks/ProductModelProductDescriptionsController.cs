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
    public class ProductModelProductDescriptionsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public ProductModelProductDescriptionsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all product model product descriptions.
        /// </summary>
        /// <returns>A list of product model product descriptions.</returns>
        // GET: api/ProductModelProductDescriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModelProductDescription>>> GetProductModelProductDescriptions()
        {
            return await _context.ProductModelProductDescriptions.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific product model product description by ID.
        /// </summary>
        /// <param name="id">The ID of the product model product description.</param>
        /// <returns>The product model product description with the specified ID.</returns>
        // GET: api/ProductModelProductDescriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModelProductDescription>> GetProductModelProductDescription(int id)
        {
            var productModelProductDescription = await _context.ProductModelProductDescriptions.FindAsync(id);

            if (productModelProductDescription == null)
            {
                return NotFound();
            }

            return productModelProductDescription;
        }

        /// <summary>
        /// Updates a specific product model product description by ID.
        /// </summary>
        /// <param name="id">The ID of the product model product description to update.</param>
        /// <param name="productModelProductDescription">The updated product model product description object.</param>
        /// <returns>No content if the update is successful.</returns>
        // PUT: api/ProductModelProductDescriptions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductModelProductDescription(int id, ProductModelProductDescription productModelProductDescription)
        {
            if (id != productModelProductDescription.ProductModelId)
            {
                return BadRequest();
            }

            _context.Entry(productModelProductDescription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductModelProductDescriptionExists(id))
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
        /// Adds a new product model product description.
        /// </summary>
        /// <param name="productModelProductDescription">The product model product description to add.</param>
        /// <returns>The newly created product model product description.</returns>
        // POST: api/ProductModelProductDescriptions
        [HttpPost]
        public async Task<ActionResult<ProductModelProductDescription>> PostProductModelProductDescription(ProductModelProductDescription productModelProductDescription)
        {
            _context.ProductModelProductDescriptions.Add(productModelProductDescription);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductModelProductDescriptionExists(productModelProductDescription.ProductModelId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProductModelProductDescription", new { id = productModelProductDescription.ProductModelId }, productModelProductDescription);
        }

        /// <summary>
        /// Deletes a specific product model product description by ID.
        /// </summary>
        /// <param name="id">The ID of the product model product description to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        // DELETE: api/ProductModelProductDescriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductModelProductDescription(int id)
        {
            var productModelProductDescription = await _context.ProductModelProductDescriptions.FindAsync(id);
            if (productModelProductDescription == null)
            {
                return NotFound();
            }

            _context.ProductModelProductDescriptions.Remove(productModelProductDescription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a product model product description with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the product model product description.</param>
        /// <returns>True if the product model product description exists, otherwise false.</returns>
        private bool ProductModelProductDescriptionExists(int id)
        {
            return _context.ProductModelProductDescriptions.Any(e => e.ProductModelId == id);
        }
    }
}
