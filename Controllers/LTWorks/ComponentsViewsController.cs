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
    public class ComponentsViewsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public ComponentsViewsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/ComponentsViews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComponentsView>>> GetComponentsViews()
        {
            return await _context.ComponentsViews.ToListAsync();
        }

        // GET: api/ComponentsViews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ComponentsView>> GetComponentsView(int id)
        {
            var componentsView = await _context.ComponentsViews.FindAsync(id);

            if (componentsView == null)
            {
                return NotFound();
            }

            return componentsView;
        }

        // PUT: api/ComponentsViews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComponentsView(int id, ComponentsView componentsView)
        {
            if (id != componentsView.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(componentsView).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComponentsViewExists(id))
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

        // POST: api/ComponentsViews
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ComponentsView>> PostComponentsView(ComponentsView componentsView)
        {
            _context.ComponentsViews.Add(componentsView);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ComponentsViewExists(componentsView.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetComponentsView", new { id = componentsView.ProductId }, componentsView);
        }

        // DELETE: api/ComponentsViews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComponentsView(int id)
        {
            var componentsView = await _context.ComponentsViews.FindAsync(id);
            if (componentsView == null)
            {
                return NotFound();
            }

            _context.ComponentsViews.Remove(componentsView);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ComponentsViewExists(int id)
        {
            return _context.ComponentsViews.Any(e => e.ProductId == id);
        }

        // GET: api/ComponentsViews/OrderByPriceAscending
        [HttpGet("OrderByPriceAscending")]
        public async Task<ActionResult<IEnumerable<ComponentsView>>> GetComponentsViewsOrderByPriceAscending()
        {
            var ComponentsViews = await _context.ComponentsViews
                                            .OrderBy(av => av.ListPrice)
                                            .ToListAsync();
            return ComponentsViews;
        }

        // GET: api/ComponentsViews/OrderByPriceDescending
        [HttpGet("OrderByPriceDescending")]
        public async Task<ActionResult<IEnumerable<ComponentsView>>> GetComponentsViewsOrderByPriceDescending()
        {
            var ComponentsViews = await _context.ComponentsViews
                                            .OrderByDescending(av => av.ListPrice)
                                            .ToListAsync();
            return ComponentsViews;
        }
    }
}
