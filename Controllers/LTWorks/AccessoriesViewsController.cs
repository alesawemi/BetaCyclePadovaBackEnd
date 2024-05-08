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
    public class AccessoriesViewsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public AccessoriesViewsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/AccessoriesViews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccessoriesView>>> GetAccessoriesViews()
        {
            return await _context.AccessoriesViews.ToListAsync();
        }

        // GET: api/AccessoriesViews/5
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

        private bool AccessoriesViewExists(int id)
        {
            return _context.AccessoriesViews.Any(e => e.ProductId == id);
        }

        // GET: api/AccessoriesViews/OrderByPriceAscending
        [HttpGet("OrderByPriceAscending")]
        public async Task<ActionResult<IEnumerable<AccessoriesView>>> GetAccessoriesViewsOrderByPriceAscending()
        {
            var accessoriesViews = await _context.AccessoriesViews
                                            .OrderBy(av => av.ListPrice)
                                            .ToListAsync();
            return accessoriesViews;
        }

        // GET: api/AccessoriesViews/OrderByPriceDescending
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
