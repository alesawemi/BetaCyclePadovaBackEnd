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
using Humanizer;

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


         
        [HttpGet("GetProductProperties")]
        public async Task<ActionResult<ProductProperties>> GetProductProperties()
        {
            ProductProperties availableOptions = new ProductProperties();

            try
            {
                //#region COLORS
                //var colors = await _context.AccessoriesViews
                //    .FromSqlRaw($"SELECT [Color]" +
                //        $"FROM [AdventureWorksLT2019].[SalesLT].[AccessoriesView]" +
                //        $"GROUP BY [Color]")
                //    .Select(x => new xProperty(x.Color))
                //    .ToListAsync();

                //foreach (var col in colors)
                //{
                //    if (col.Option is not null) { availableOptions.availableColors.Add(col.Option); }
                //    else { availableOptions.availableColors.Add("other"); }
                //}
                //#endregion

                //#region CATEGORIES
                //var categories = await _context.AccessoriesViews
                //    .FromSqlRaw($"SELECT [ProductCategory]" +
                //        $"FROM [AdventureWorksLT2019].[SalesLT].[AccessoriesView]" +
                //        $"GROUP BY [ProductCategory]")
                //    .Select(x => new xProperty(x.ProductCategory))
                //    .ToListAsync();

                //foreach (var cat in categories) { availableOptions.availableCategories.Add(cat.Option); }
                //#endregion

                //#region PRICE
                //var result = await _context.AccessoriesViews
                //   .FromSql($"SELECT * FROM [AdventureWorksLT2019].[SalesLT].[AccessoriesView]")
                //   .ToListAsync();

                //foreach (var r in result)
                //{
                //    if (r.ListPrice > availableOptions.priceMax) availableOptions.priceMax = r.ListPrice;
                //    if (r.ListPrice < availableOptions.priceMin) availableOptions.priceMin = r.ListPrice;
                //}
                //#endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }           

            return availableOptions;
        }


        [Route("GetWithFilters")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<AccessoriesView>>> GetAccessoriesViewsWithFilters(ProductFilters Filters) 
        {
            

            var query = _context.AccessoriesViews
                   .FromSql($"SELECT * FROM [AdventureWorksLT2019].[SalesLT].[AccessoriesView]");

            if (Filters.descPrice) { query = _context.AccessoriesViews.OrderByDescending(av => av.ListPrice); }
            if (Filters.ascPrice) { query = _context.AccessoriesViews.OrderBy(av => av.ListPrice); }

            if (Filters.productName != "allProducts") { query = query.Where(a => a.ProductName.Contains(Filters.productName)); }

            if (Filters.color != "color") {                
                string[] colors = Filters.color.Split(":");
                if (colors[1] == "other") { query = query.Where(a => a.Color == null); }
                else { query = query.Where(a => a.Color == colors[1]); }                                              
            }

            if (Filters.size != "size") { query = query.Where(a => a.Size == Filters.size); }
            
            if (Filters.productCategory != "category") {
                string[] categories = Filters.productCategory.Split(":");
                query = query.Where(a => a.ProductCategory == categories[1]); 
            }

            if (Filters.maxWeight != 0) { query = query.Where(a => a.Weight < Filters.maxWeight); }

            if (Filters.minWeight != 0) { query = query.Where(a => a.Weight > Filters.minWeight); }

            if (Filters.maxPrice != 0) { query = query.Where(a => a.ListPrice <= Filters.maxPrice); }

            if (Filters.minPrice != 0) { query = query.Where(a => a.ListPrice > Filters.minPrice); }            

            var filtered = await query
                .ToListAsync();

            return filtered;
        }
    }
}
