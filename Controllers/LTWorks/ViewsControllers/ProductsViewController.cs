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

namespace BetaCycle_Padova.Controllers.LTWorks.ViewsControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsViewController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public ProductsViewController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        [HttpGet("GetByParam/{param}")]
        public async Task<ActionResult<IEnumerable<ProductsView>>> GetByParam(string param)
        {
            return await _context.ProductsView.FromSqlRaw(
                        $"SELECT * FROM [dbo].[vProductsView] WHERE " +
                        $"[Name] LIKE '%{param}%' OR " +
                        $"[Color] LIKE '%{param}%' OR " +
                        $"[Size] LIKE '%{param}%' OR " +
                        $"[ProductCategory] LIKE '%{param}%' OR " +
                        $"[ProductModel] LIKE '%{param}%'"
                    ).ToListAsync();
            
        }


        
    }
}
