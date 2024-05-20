using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Models.LTWorks;
using BetaCycle_Padova.Models.LTWorks.Filters;
using Microsoft.Data.SqlClient;
using DnsClient;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.CompilerServices;

namespace BetaCycle_Padova.Controllers.LTWorks
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



        [HttpGet("GetProductProperties")]
        public async Task<ActionResult<ProductProperties>> GetProductProperties()
        {
            ProductProperties availableOptions = new ProductProperties();

            try
            {
                #region COLORS
                var colors = await _context.avOptions
                    .FromSqlRaw($"SELECT [Color] FROM [AdventureWorksLT2019].[SalesLT].[BikesView]" +
                        $"GROUP BY [Color]")
                    .ToListAsync();

                foreach (var col in colors)
                {
                    if (col.Option is not null) { availableOptions.availableColors.Add(col.Option); }
                    else { availableOptions.availableColors.Add("other"); }
                }
                #endregion

                #region CATEGORIES
                var categories = await _context.avOptions
                    .FromSqlRaw($"SELECT [ProductCategory] FROM [AdventureWorksLT2019].[SalesLT].[BikesView]" +
                        $"GROUP BY [ProductCategory]")
                    .ToListAsync();

                foreach (var cat in categories)
                {
                    if (cat.Option is not null) { availableOptions.availableCategories.Add(cat.Option); }
                    else { availableOptions.availableCategories.Add("other"); }
                }
                #endregion

                #region SIZES
                var size = await _context.avOptions
                    .FromSqlRaw($"SELECT [Size] FROM [AdventureWorksLT2019].[SalesLT].[BikesView]" +
                        $"GROUP BY [Size]")
                    .ToListAsync();

                foreach (var s in size)
                {
                    if (s.Option is not null) { availableOptions.availableSizes.Add(s.Option); }
                    else { availableOptions.availableSizes.Add("other"); }
                }
                #endregion

                #region PRICE and WEIGHT
                var MaxMin = await _context.PriceAndWeightOptions
                    .FromSqlRaw($"SELECT MAX(ListPrice) AS MaxP, " +
                        $"MIN(ListPrice) AS MinP, " +
                        $"MAX([Weight]) AS MaxW, " +
                        $"MIN([Weight]) AS MinW " +
                        $"FROM[AdventureWorksLT2019].[SalesLT].[BikesView]")
                    .FirstOrDefaultAsync();

                availableOptions.PriceAndWeight = MaxMin;
                //availableOptions.priceMax = MaxMin[0].MaxP;
                //availableOptions.priceMin = MaxMin[0].MinP;
                //availableOptions.weightMax = MaxMin[0].MaxW;
                //availableOptions.weightMin = MaxMin[0].MinW;
                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return availableOptions;
        }



        [Route("GetWithFilters")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<BikesView>>> GetBikesViewsWithFilters(ProductFilters Filters)
        {
            var tableName = "[AdventureWorksLT2019].[SalesLT].[BikesView]";

            var whereClauses = new List<string>();
            var parameters = new List<object>();

            if (Filters.productName != "allProducts")
            {
                whereClauses.Add("[ProductName] LIKE {0}");
                parameters.Add($"%{Filters.productName}%");
            }

            if (Filters.color != "color")
            {
                var colors = Filters.color.Split(":").Skip(1).ToList();
                if (colors.Count > 0)
                {
                    var colorConditions = new List<string>();
                    foreach (var col in colors)
                    {
                        if (col == "other")
                            colorConditions.Add("[Color] IS NULL");
                        else
                        {
                            colorConditions.Add($"[Color] = {{{parameters.Count}}}");
                            parameters.Add(col);
                        }
                    }
                    whereClauses.Add($"({string.Join(" OR ", colorConditions)})");
                }
            }

            if (Filters.productCategory != "category")
            {
                var categories = Filters.productCategory.Split(":").Skip(1).ToList();
                if (categories.Count > 0)
                {
                    var categoryConditions = new List<string>();
                    foreach (var cat in categories)
                    {
                        if (cat == "other")
                            categoryConditions.Add("[ProductCategory] IS NULL");
                        else
                        {
                            categoryConditions.Add($"[ProductCategory] = {{{parameters.Count}}}");
                            parameters.Add(cat);
                        }
                    }
                    whereClauses.Add($"({string.Join(" OR ", categoryConditions)})");
                }
            }

            if (Filters.size != "size")
            {
                var sizes = Filters.size.Split(":").Skip(1).ToList();
                if (sizes.Count > 0)
                {
                    var sizeConditions = new List<string>();
                    foreach (var s in sizes)
                    {
                        if (s == "other")
                            sizeConditions.Add("[Size] IS NULL");
                        else
                        {
                            sizeConditions.Add($"[Size] = {{{parameters.Count}}}");
                            parameters.Add(s);
                        }
                    }
                    whereClauses.Add($"({string.Join(" OR ", sizeConditions)})");
                }
            }

            if (Filters.pIntervals.Count > 0)
            {
                var priceConditions = new List<string>();
                foreach (var pI in Filters.pIntervals)
                {
                    var conditionParts = new List<string>();
                    if (pI.max > 0)
                    {
                        conditionParts.Add($"[ListPrice] <= {{{parameters.Count}}}");
                        parameters.Add(pI.max);
                    }
                    if (pI.min > 0)
                    {
                        conditionParts.Add($"[ListPrice] > {{{parameters.Count}}}");
                        parameters.Add(pI.min);
                    }
                    if (conditionParts.Any())
                    {
                        priceConditions.Add($"({string.Join(" AND ", conditionParts)})");
                    }
                }
                if (priceConditions.Any())
                {
                    whereClauses.Add($"({string.Join(" OR ", priceConditions)})");
                }
            }

            if (Filters.wIntervals.Count > 0)
            {
                var weightConditions = new List<string>();
                foreach (var wI in Filters.wIntervals)
                {
                    var conditionParts = new List<string>();
                    if (wI.max > 0)
                    {
                        conditionParts.Add($"[Weight] <= {{{parameters.Count}}}");
                        parameters.Add(wI.max);
                    }
                    if (wI.min > 0)
                    {
                        conditionParts.Add($"[Weight] > {{{parameters.Count}}}");
                        parameters.Add(wI.min);
                    }
                    if (conditionParts.Any())
                    {
                        weightConditions.Add($"({string.Join(" AND ", conditionParts)})");
                    }
                }
                if (weightConditions.Any())
                {
                    whereClauses.Add($"({string.Join(" OR ", weightConditions)})");
                }
            }

            var queryString = $"SELECT * FROM {tableName} ";

            if (whereClauses.Any())
                queryString += "WHERE " + string.Join(" AND ", whereClauses);

            if (Filters.ascPrice)
            {
                queryString += " ORDER BY [ListPrice]";
            }
            else if (Filters.descPrice)
            {
                queryString += " ORDER BY [ListPrice] DESC";
            }

            var sqlInterpolatedString = FormattableStringFactory.Create(queryString, parameters.ToArray());

            var query = _context.BikesViews
                .FromSql(sqlInterpolatedString);

            var filtered = await query.ToListAsync();
            return filtered;
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
