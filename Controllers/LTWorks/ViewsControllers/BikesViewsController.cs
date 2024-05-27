﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Models.LTWorks.OptionsAndFilters;
using Microsoft.Data.SqlClient;
using DnsClient;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.CompilerServices;
using BetaCycle_Padova.Models.LTWorks.Views;

namespace BetaCycle_Padova.Controllers.LTWorks.ViewsControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BikesViewsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public BikesViewsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/BikesViews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BikesView>>> GetBikesViews()
        {
            return await _context.BikesViews.ToListAsync();
        }

        // GET: api/BikesViews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BikesView>> GetBikesView(int id)
        {
            var bikesView = await _context.BikesViews.FindAsync(id);

            if (bikesView == null)
            {
                return NotFound();
            }

            return bikesView;
        }

        // PUT: api/BikesViews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBikesView(int id, BikesView bikesView)
        {
            if (id != bikesView.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(bikesView).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BikesViewExists(id))
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

        // POST: api/BikesViews
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BikesView>> PostBikesView(BikesView bikesView)
        {
            _context.BikesViews.Add(bikesView);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BikesViewExists(bikesView.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBikesView", new { id = bikesView.ProductId }, bikesView);
        }

        // DELETE: api/BikesViews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBikesView(int id)
        {
            var bikesView = await _context.BikesViews.FindAsync(id);
            if (bikesView == null)
            {
                return NotFound();
            }

            _context.BikesViews.Remove(bikesView);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        private bool BikesViewExists(int id)
        {
            return _context.BikesViews.Any(e => e.ProductId == id);
        }



        // GET: api/BikesViews/OrderByPriceAscending
        [HttpGet("OrderByPriceAscending")]
        public async Task<ActionResult<IEnumerable<BikesView>>> GetBikesViewsOrderByPriceAscending()
        {
            var BikesViews = await _context.BikesViews
                                            .OrderBy(av => av.ListPrice)
                                            .ToListAsync();
            return BikesViews;
        }

        // GET: api/BikesViews/OrderByPriceDescending
        [HttpGet("OrderByPriceDescending")]
        public async Task<ActionResult<IEnumerable<BikesView>>> GetBikesViewsOrderByPriceDescending()
        {
            var BikesViews = await _context.BikesViews
                                            .OrderByDescending(av => av.ListPrice)
                                            .ToListAsync();
            return BikesViews;
        }

        #region PROVA: più veloce SP o "query diretta" --> se vuoi provare vedi riga #20 AW2019 Context
        ////Executed DbCommand(7ms)
        ////[Parameters=[], CommandType='Text', CommandTimeout='30']
        ////SELECT[b].[Color] FROM(SELECT[Color] FROM [AdventureWorksLT2019].[SalesLT].[BikesView] GROUP BY [Color]      ) AS[b]
        //[HttpGet("GetColors")]
        //public async Task<ActionResult<ProductProperties>> GetColors()
        //{
        //    ProductProperties availableOptions = new ProductProperties();
        //    try
        //    {
        //        var colors = await _context.BikesViews
        //            .FromSqlRaw($"SELECT [Color] FROM [AdventureWorksLT2019].[SalesLT].[BikesView]" +
        //                $"GROUP BY [Color]")
        //            .Select(x => new xProperty(x.Color))
        //            .ToListAsync();

        //        foreach (var col in colors)
        //        {
        //            if (col.Option is not null) { availableOptions.availableColors.Add(col.Option); }
        //            else { availableOptions.availableColors.Add("other"); }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //    return availableOptions;
        //}

        ////Executed DbCommand (7ms)
        ////[Parameters=[], CommandType='Text', CommandTimeout='30']
        ////EXECUTE [dbo].[sp_GetProductProperties_AvailableColors]
        //[HttpGet("GetColorsSP")]
        //public async Task<ActionResult<ProductProperties>> GetColorsSP()
        //{
        //    ProductProperties availableOptions = new ProductProperties();
        //    try
        //    {
        //        var colors = await _context.availableOptions
        //            .FromSqlRaw($"EXECUTE [dbo].[sp_GetProductProperties_AvailableColors]")
        //            .ToListAsync();
        //        foreach (var col in colors)
        //        {
        //            if (col.Option is not null) { availableOptions.availableColors.Add(col.Option); }
        //            else { availableOptions.availableColors.Add("other"); }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //    return availableOptions;
        //}
        #endregion

    }
}
