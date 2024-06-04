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
    public class SalesOrderDetailsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public SalesOrderDetailsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all sales order details.
        /// </summary>
        /// <returns>A list of sales order details.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderDetail>>> GetSalesOrderDetails()
        {
            return await _context.SalesOrderDetails.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific sales order detail by ID.
        /// </summary>
        /// <param name="id">The ID of the sales order detail.</param>
        /// <returns>The sales order detail with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderDetail>> GetSalesOrderDetail(int id)
        {
            var salesOrderDetail = await _context.SalesOrderDetails.FindAsync(id);

            if (salesOrderDetail == null)
            {
                return NotFound();
            }

            return salesOrderDetail;
        }

        /// <summary>
        /// Updates a specific sales order detail by ID.
        /// </summary>
        /// <param name="id">The ID of the sales order detail to update.</param>
        /// <param name="salesOrderDetail">The updated sales order detail object.</param>
        /// <returns>No content if the update is successful.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderDetail(int id, SalesOrderDetail salesOrderDetail)
        {
            if (id != salesOrderDetail.SalesOrderId)
            {
                return BadRequest();
            }

            _context.Entry(salesOrderDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesOrderDetailExists(id))
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
        /// Adds a new sales order detail.
        /// </summary>
        /// <param name="salesOrderDetail">The sales order detail to add.</param>
        /// <returns>The newly created sales order detail.</returns>
        [HttpPost]
        public async Task<ActionResult<SalesOrderDetail>> PostSalesOrderDetail(SalesOrderDetail salesOrderDetail)
        {
            _context.SalesOrderDetails.Add(salesOrderDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SalesOrderDetailExists(salesOrderDetail.SalesOrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSalesOrderDetail", new { id = salesOrderDetail.SalesOrderId }, salesOrderDetail);
        }

        /// <summary>
        /// Deletes a specific sales order detail by ID.
        /// </summary>
        /// <param name="id">The ID of the sales order detail to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrderDetail(int id)
        {
            var salesOrderDetail = await _context.SalesOrderDetails.FindAsync(id);
            if (salesOrderDetail == null)
            {
                return NotFound();
            }

            _context.SalesOrderDetails.Remove(salesOrderDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a sales order detail with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the sales order detail.</param>
        /// <returns>True if the sales order detail exists, otherwise false.</returns>
        private bool SalesOrderDetailExists(int id)
        {
            return _context.SalesOrderDetails.Any(e => e.SalesOrderId == id);
        }
    }
}
