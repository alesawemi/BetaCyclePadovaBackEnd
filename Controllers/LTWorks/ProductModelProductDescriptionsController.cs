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

        // GET: api/ProductModelProductDescriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModelProductDescription>>> GetProductModelProductDescriptions()
        {
            return await _context.ProductModelProductDescriptions.ToListAsync();
        }

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

        // PUT: api/ProductModelProductDescriptions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/ProductModelProductDescriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        private bool ProductModelProductDescriptionExists(int id)
        {
            return _context.ProductModelProductDescriptions.Any(e => e.ProductModelId == id);
        }
    }
}
