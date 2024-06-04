using System;
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
using BetaCycle_Padova.Models.LTWorks;
using NLog;

namespace BetaCycle_Padova.Controllers.LTWorks.ViewsControllers
{
    /// <summary>
    /// Controller for handling product views.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsViewController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public ProductsViewController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        private static Logger ProductsViewNlogLogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Retrieves products matching the given parameter from the view.
        /// </summary>
        /// <param name="param">The search parameter.</param>
        [HttpGet("GetByParam/{param}")]
        public async Task<ActionResult<IEnumerable<ProductsView>>> GetByParam(string param)
        {
            try
            {
                ProductsViewNlogLogger.Info("Products View Controller - GetByParam");

                return await _context.ProductsView.FromSqlRaw(
                                        $"SELECT * FROM [dbo].[vProductsView] WHERE " +
                                        $"[ProductName] LIKE '%{param}%' OR " +
                                        $"[Color] LIKE '%{param}%' OR " +
                                        $"[Size] LIKE '%{param}%' OR " +
                                        $"[ProductCategory] LIKE '%{param}%' OR " +
                                        $"[ProductModel] LIKE '%{param}%'"
                                    ).ToListAsync();
            }
            catch (Exception ex)
            {
                ProductsViewNlogLogger.Error("Products View Controller - Exception thrown from " +
                    "GetByParam");

                return BadRequest(new { message = "Exception thrown from GetByParam in Products View Controller." });
            }
        }
    }
}
