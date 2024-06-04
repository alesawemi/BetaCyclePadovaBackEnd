using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Models.LTWorks.OptionsAndFilters;
using Humanizer;
using BetaCycle_Padova.Models.LTWorks.Views;
using Microsoft.AspNetCore.Authorization;

namespace BetaCycle_Padova.Controllers.LTWorks.ViewsControllers
{
    /// <summary>
    /// API Controller for managing views of accessories.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccessoriesViewsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessoriesViewsController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public AccessoriesViewsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/AccessoriesViews
        /// <summary>
        /// Retrieves all accessories views.
        /// </summary>
        /// <returns>A list of accessories views.</returns>
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccessoriesView>>> GetAccessoriesViews()
        {
            return await _context.AccessoriesViews.ToListAsync();
        }

        // GET: api/AccessoriesViews/5
        /// <summary>
        /// Retrieves a specific accessories view by its ID.
        /// </summary>
        /// <param name="id">The ID of the accessories view to retrieve.</param>
        /// <returns>The requested accessories view.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AccessoriesView>> GetAccessoriesView(int id)
        {
            var accessoriesView = await _context.AccessoriesViews.FindAsync(id);

            if (accessoriesView == null)
            {
                return NotFound();
            }

            return accessoriesView;
        }

        // PUT: api/AccessoriesViews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates an accessories view.
        /// </summary>
        /// <param name="id">The ID of the accessories view to update.</param>
        /// <param name="accessoriesView">The updated accessories view.</param>
        /// <returns>An action result indicating the outcome of the operation.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccessoriesView(int id, AccessoriesView accessoriesView)
        {
            if (id != accessoriesView.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(accessoriesView).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccessoriesViewExists(id))
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

        // POST: api/AccessoriesViews
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Creates a new accessories view.
        /// </summary>
        /// <param name="accessoriesView">The accessories view to create.</param>
        /// <returns>An action result indicating the outcome of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<AccessoriesView>> PostAccessoriesView(AccessoriesView accessoriesView)
        {
            _context.AccessoriesViews.Add(accessoriesView);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AccessoriesViewExists(accessoriesView.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAccessoriesView", new { id = accessoriesView.ProductId }, accessoriesView);
        }

        // DELETE: api/AccessoriesViews/5
        /// <summary>
        /// Deletes an accessories view.
        /// </summary>
        /// <param name="id">The ID of the accessories view to delete.</param>
        /// <returns>An action result indicating the outcome of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccessoriesView(int id)
        {
            var accessoriesView = await _context.AccessoriesViews.FindAsync(id);
            if (accessoriesView == null)
            {
                return NotFound();
            }

            _context.AccessoriesViews.Remove(accessoriesView);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Additional methods...

        private bool AccessoriesViewExists(int id)
        {
            return _context.AccessoriesViews.Any(e => e.ProductId == id);
        }

        // GET: api/AccessoriesViews/OrderByPriceAscending
        // GET: api/AccessoriesViews/OrderByPriceDescending
        // Additional methods...

        // GET: api/AccessoriesViews/OrderByPriceAscending
        /// <summary>
        /// Retrieves all accessories views ordered by price in ascending order.
        /// </summary>
        /// <returns>A list of accessories views ordered by price in ascending order.</returns>
        [HttpGet("OrderByPriceAscending")]
        public async Task<ActionResult<IEnumerable<AccessoriesView>>> GetAccessoriesViewsOrderByPriceAscending()
        {
            var accessoriesViews = await _context.AccessoriesViews
                                            .OrderBy(av => av.ListPrice)
                                            .ToListAsync();
            return accessoriesViews;
        }

        // GET: api/AccessoriesViews/OrderByPriceDescending
        /// <summary>
        /// Retrieves all accessories views ordered by price in descending order.
        /// </summary>
        /// <returns>A list of accessories views ordered by price in descending order.</returns>
        [HttpGet("OrderByPriceDescending")]
        public async Task<ActionResult<IEnumerable<AccessoriesView>>> GetAccessoriesViewsOrderByPriceDescending()
        {
            var accessoriesViews = await _context.AccessoriesViews
                                            .OrderByDescending(av => av.ListPrice)
                                            .ToListAsync();
            return accessoriesViews;
        }
    }
}
