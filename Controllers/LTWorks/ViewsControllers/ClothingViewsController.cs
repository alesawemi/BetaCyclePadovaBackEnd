using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Models.LTWorks.Views;

namespace BetaCycle_Padova.Controllers.LTWorks.ViewsControllers
{
    /// <summary>
    /// API Controller for managing views of clothing.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClothingViewsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClothingViewsController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ClothingViewsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/ClothingViews
        /// <summary>
        /// Retrieves all clothing views.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClothingView>>> GetClothingViews()
        {
            return await _context.ClothingViews.ToListAsync();
        }

        // GET: api/ClothingViews/5
        /// <summary>
        /// Retrieves a specific clothing view by its ID.
        /// </summary>
        /// <param name="id">The ID of the clothing view to retrieve.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClothingView>> GetClothingView(int id)
        {
            var clothingView = await _context.ClothingViews.FindAsync(id);

            if (clothingView == null)
            {
                return NotFound();
            }

            return clothingView;
        }

        // PUT: api/ClothingViews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates a clothing view.
        /// </summary>
        /// <param name="id">The ID of the clothing view to update.</param>
        /// <param name="clothingView">The updated clothing view.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClothingView(int id, ClothingView clothingView)
        {
            if (id != clothingView.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(clothingView).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClothingViewExists(id))
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

        // POST: api/ClothingViews
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Creates a new clothing view.
        /// </summary>
        /// <param name="clothingView">The clothing view to create.</param>
        [HttpPost]
        public async Task<ActionResult<ClothingView>> PostClothingView(ClothingView clothingView)
        {
            _context.ClothingViews.Add(clothingView);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ClothingViewExists(clothingView.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetClothingView", new { id = clothingView.ProductId }, clothingView);
        }

        // DELETE: api/ClothingViews/5
        /// <summary>
        /// Deletes a clothing view.
        /// </summary>
        /// <param name="id">The ID of the clothing view to delete.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClothingView(int id)
        {
            var clothingView = await _context.ClothingViews.FindAsync(id);
            if (clothingView == null)
            {
                return NotFound();
            }

            _context.ClothingViews.Remove(clothingView);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClothingViewExists(int id)
        {
            return _context.ClothingViews.Any(e => e.ProductId == id);
        }

        // GET: api/ClothingViews/OrderByPriceAscending
        /// <summary>
        /// Retrieves all clothing views ordered by price in ascending order.
        /// </summary>
        [HttpGet("OrderByPriceAscending")]
        public async Task<ActionResult<IEnumerable<ClothingView>>> GetClothingViewsOrderByPriceAscending()
        {
            var ClothingViews = await _context.ClothingViews
                                            .OrderBy(av => av.ListPrice)
                                            .ToListAsync();
            return ClothingViews;
        }

        // GET: api/ClothingViews/OrderByPriceDescending
        /// <summary>
        /// Retrieves all clothing views ordered by price in descending order.
        /// </summary>
        [HttpGet("OrderByPriceDescending")]
        public async Task<ActionResult<IEnumerable<ClothingView>>> GetClothingViewsOrderByPriceDescending()
        {
            var ClothingViews = await _context.ClothingViews
                                            .OrderByDescending(av => av.ListPrice)
                                            .ToListAsync();
            return ClothingViews;
        }
    }
}
