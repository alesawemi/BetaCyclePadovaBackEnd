using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Models.LTWorks;
using NLog;

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

        private static Logger OrderHeaderNlogLogger = LogManager.GetCurrentClassLogger();

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
            try
            {
                _context.SalesOrderHeaders.Add(salesOrderHeader);
                await _context.SaveChangesAsync();

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(new { message = "an exception occured in the backend! " });
            }


            return CreatedAtAction("GetSalesOrderHeader", new { id = salesOrderHeader.SalesOrderId }, salesOrderHeader);
        }

        /// <summary>
        /// Deletes a specific sales order header by ID.
        /// </summary>
        /// <param name="id">The ID of the sales order header to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>


        // POST: api/SalesOrderHeaders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("FrontEnd")]
        public async Task<ActionResult<SalesOrderHeader>> PostSalesOrderHeaderFE(SalesOrderHeaderFE salesOrderHeaderFE)
        {
            Console.WriteLine("Sei dentro Sales Order Header");
            try
            {
                OrderHeaderNlogLogger.Info("Sales Order Header Controller - Post Front End");
                SalesOrderHeader salesOrderHeader = new()
                {
                    SalesOrderId = salesOrderHeaderFE.SalesOrderId,
                    RevisionNumber = salesOrderHeaderFE.RevisionNumber,
                    OrderDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(12), //ricorda di mettere un +12
                    ShipDate = DateTime.Now.AddDays(7), //ricorda di mettere un +7
                    Status = salesOrderHeaderFE.Status,
                    OnlineOrderFlag = salesOrderHeaderFE.OnlineOrderFlag,
                    SalesOrderNumber = salesOrderHeaderFE.SalesOrderNumber,
                    PurchaseOrderNumber = salesOrderHeaderFE.PurchaseOrderNumber,
                    AccountNumber = salesOrderHeaderFE.AccountNumber,
                    CustomerId = salesOrderHeaderFE.CustomerId,
                    ShipToAddressId = salesOrderHeaderFE.ShipToAddressId,
                    BillToAddressId = salesOrderHeaderFE.BillToAddressId,
                    ShipMethod = salesOrderHeaderFE.ShipMethod,
                    CreditCardApprovalCode = salesOrderHeaderFE.CreditCardApprovalCode,
                    SubTotal = salesOrderHeaderFE.SubTotal,
                    TaxAmt = salesOrderHeaderFE.TaxAmt,
                    Freight = salesOrderHeaderFE.Freight,
                    TotalDue = salesOrderHeaderFE.TotalDue,
                    Comment = salesOrderHeaderFE.Comment,                   
                };

                _context.SalesOrderHeaders.Add(salesOrderHeader);
                await _context.SaveChangesAsync();

                ////adesso salesOrderHeader è stato aggiornato
                //// Debugging log for SalesOrderHeader ID
                //Console.WriteLine($"SalesOrderHeader ID: {salesOrderHeader.SalesOrderId}");

                //Console.WriteLine("DETAILS N: "+ salesOrderHeaderFE.SalesOrderDetails.Count);

                //Console.ReadKey();
               
                ////poi mi ricavo la lista di SalesOrderDetail
                //List<SalesOrderDetail> salesOrderDetail = [];
                //salesOrderDetail = salesOrderHeaderFE.SalesOrderDetails.Select(detail => new SalesOrderDetail
                //{
                //    SalesOrderId = salesOrderHeader.SalesOrderId, //l'id dell'header foreign key
                //    SalesOrderDetailId = detail.SalesOrderDetailId,
                //    OrderQty = detail.OrderQty,
                //    ProductId = detail.ProductId,
                //    UnitPrice = detail.UnitPrice,
                //    UnitPriceDiscount = detail.UnitPriceDiscount,
                //    LineTotal = detail.LineTotal
                //}).ToList();

                //// Debugging log for details
                //Console.WriteLine($"Number of SalesOrderDetails to add: {salesOrderDetail.Count}");

                ////e poi aggiungere al database
                //foreach (SalesOrderDetail detail in salesOrderDetail)
                //{
                //    Console.WriteLine($"Detail - SalesOrderId: {detail.SalesOrderId}, ProductId: {detail.ProductId}, OrderQty: {detail.OrderQty}");

                //    _context.SalesOrderDetails.Add(detail);                    
                //}
                //await _context.SaveChangesAsync();

                return CreatedAtAction("GetSalesOrderHeader", new { id = salesOrderHeader.SalesOrderId }, salesOrderHeader);
            }

            catch (DbUpdateException ex) {
                Console.WriteLine(ex);
                OrderHeaderNlogLogger.Error(ex, "Sales Order Header Controller - Post Front End");
                return BadRequest(new { message = "an exception occured in the DB! " });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                OrderHeaderNlogLogger.Error(ex, "Sales Order Header Controller - Post Front End");
                return BadRequest(new { message = "an exception occured in the backend! " });
            }


        }




        // DELETE: api/SalesOrderHeaders/5
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
