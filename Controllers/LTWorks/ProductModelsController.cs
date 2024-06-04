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
    public class ProductModelsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public ProductModelsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all product models.
        /// </summary>
        /// <returns>A list of product models.</returns>
        // GET: api/ProductModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProductModels()
        {
            return await _context.ProductModels.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific product model by ID.
        /// </summary>
        /// <param name="id">The ID of the product model.</param>
        /// <returns>The product model with the specified ID.</returns>
        // GET: api/ProductModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProductModel(int id)
        {
            var productModel = await _context.ProductModels.FindAsync(id);

            if (productModel == null)
            {
                return NotFound();
            }

            return productModel;
        }

        /// <summary>
        /// Updates a specific product model by ID.
        /// </summary>
        /// <param name="id">The ID of the product model to update.</param>
        /// <param name="productModel">The updated product model object.</param>
        /// <returns>No content if the update is successful.</returns>
        // PUT: api/ProductModels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductModel(int id, ProductModel productModel)
        {
            if (id != productModel.ProductModelId)
            {
                return BadRequest();
            }

            _context.Entry(productModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductModelExists(id))
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
        /// Adds a new product model.
        /// </summary>
        /// <param name="productModel">The product model to add.</param>
        /// <returns>The newly created product model.</returns>
        // POST: api/ProductModels
        [HttpPost]
        public async Task<ActionResult<ProductModel>> PostProductModel(ProductModel productModel)
        {
            _context.ProductModels.Add(productModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductModel", new { id = productModel.ProductModelId }, productModel);
        }

        /// <summary>
        /// Deletes a specific product model by ID.
        /// </summary>
        /// <param name="id">The ID of the product model to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        // DELETE: api/ProductModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductModel(int id)
        {
            var productModel = await _context.ProductModels.FindAsync(id);
            if (productModel == null)
            {
                return NotFound();
            }

            _context.ProductModels.Remove(productModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a product model with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the product model.</param>
        /// <returns>True if the product model exists, otherwise false.</returns>
        private bool ProductModelExists(int id)
        {
            return _context.ProductModels.Any(e => e.ProductModelId == id);
        }
    }
}
