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

        // GET: api/ProductDescriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDescription>>> GetProductDescriptions()
        {
            return await _context.ProductDescriptions.ToListAsync();
        }

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

        // PUT: api/ProductDescriptions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/ProductDescriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductDescription>> PostProductDescription(ProductDescription productDescription)
        {
            _context.ProductDescriptions.Add(productDescription);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductDescription", new { id = productDescription.ProductDescriptionId }, productDescription);
        }

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

        private bool ProductDescriptionExists(int id)
        {
            return _context.ProductDescriptions.Any(e => e.ProductDescriptionId == id);
        }
    }
}
