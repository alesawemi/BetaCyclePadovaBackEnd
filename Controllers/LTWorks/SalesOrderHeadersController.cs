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
    public class SalesOrderHeadersController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public SalesOrderHeadersController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all sales order headers.
        /// </summary>
        /// <returns>A list of sales order headers.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderHeader>>> GetSalesOrderHeaders()
        {
            return await _context.SalesOrderHeaders.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific sales order header by ID.
        /// </summary>
        /// <param name="id">The ID of the sales order header.</param>
        /// <returns>The sales order header with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderHeader>> GetSalesOrderHeader(int id)
        {
            var salesOrderHeader = await _context.SalesOrderHeaders.FindAsync(id);

            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            return salesOrderHeader;
        }

        /// <summary>
        /// Updates a specific sales order header by ID.
        /// </summary>
        /// <param name="id">The ID of the sales order header to update.</param>
        /// <param name="salesOrderHeader">The updated sales order header object.</param>
        /// <returns>No content if the update is successful.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderHeader(int id, SalesOrderHeader salesOrderHeader)
        {
            if (id != salesOrderHeader.SalesOrderId)
            {
                return BadRequest();
            }

            _context.Entry(salesOrderHeader).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesOrderHeaderExists(id))
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
        /// Adds a new sales order header.
        /// </summary>
        /// <param name="salesOrderHeader">The sales order header to add.</param>
        /// <returns>The newly created sales order header.</returns>
        [HttpPost]
        public async Task<ActionResult<SalesOrderHeader>> PostSalesOrderHeader(SalesOrderHeader salesOrderHeader)
        {
            _context.SalesOrderHeaders.Add(salesOrderHeader);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSalesOrderHeader", new { id = salesOrderHeader.SalesOrderId }, salesOrderHeader);
        }

        /// <summary>
        /// Deletes a specific sales order header by ID.
        /// </summary>
        /// <param name="id">The ID of the sales order header to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrderHeader(int id)
        {
            var salesOrderHeader = await _context.SalesOrderHeaders.FindAsync(id);
            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            _context.SalesOrderHeaders.Remove(salesOrderHeader);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a sales order header with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the sales order header.</param>
        /// <returns>True if the sales order header exists, otherwise false.</returns>
        private bool SalesOrderHeaderExists(int id)
        {
            return _context.SalesOrderHeaders.Any(e => e.SalesOrderId == id);
        }
    }
}
