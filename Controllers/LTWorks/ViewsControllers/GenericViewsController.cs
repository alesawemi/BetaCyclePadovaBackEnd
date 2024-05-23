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

namespace BetaCycle_Padova.Controllers.LTWorks.ViewsControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericViewsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public GenericViewsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenericView>>> GetAllItemsFromView([FromQuery] string view)
        {
            string ViewName = string.Empty;

            switch (view)
            {
                case "clothing":
                    ViewName = "[ClothingView]";
                    break;
            }

            string fromTable = $"[AdventureWorksLT2019].[SalesLT].{ViewName}";

            return await _context.GenericViews
                    .FromSqlRaw($"SELECT * FROM {fromTable}").ToListAsync();
        }

        [HttpGet("GetProperties")]
        public async Task<ActionResult<ProductProperties>> GetProductPropertiesFromView([FromQuery] string view)
        {
            string ViewName = string.Empty;

            switch (view)
            {
                case "clothing":
                    ViewName = "[ClothingView]";
                    break;
            }

            string fromTable = $"[AdventureWorksLT2019].[SalesLT].{ViewName}";

            ProductProperties availableOptions = new ProductProperties();

            try
            {
                #region COLORS
                var colors = await _context.avOptions
                    .FromSqlRaw($"SELECT COALESCE([Color], 'other') FROM {fromTable}" +
                        $"GROUP BY [Color]")
                    .ToListAsync();

                foreach (var col in colors)
                {
                    if (!string.IsNullOrEmpty(col.Option)) { availableOptions.availableColors.Add(col.Option); }
                    else { availableOptions.availableColors.Add("other"); }
                }
                #endregion

                #region CATEGORIES
                var categories = await _context.avOptions
                    .FromSqlRaw($"SELECT COALESCE([ProductCategory], 'other') FROM {fromTable}" +
                        $"GROUP BY [ProductCategory]")
                    .ToListAsync();

                foreach (var cat in categories)
                {
                    if (!string.IsNullOrEmpty(cat.Option)) { availableOptions.availableCategories.Add(cat.Option); }
                    else { availableOptions.availableCategories.Add("other"); }
                }
                #endregion

                #region SIZES
                var size = await _context.avOptions
                    .FromSqlRaw($"SELECT COALESCE([Size], 'other') FROM {fromTable}" +
                        $"GROUP BY [Size]")
                    .ToListAsync();

                foreach (var s in size)
                {
                    if (!string.IsNullOrEmpty(s.Option)) { availableOptions.availableSizes.Add(s.Option); }
                    else { availableOptions.availableSizes.Add("other"); }
                }
                #endregion

                #region PRICE and WEIGHT
                var MaxMin = await _context.PriceAndWeightOptions
                    .FromSqlRaw($"SELECT COALESCE(MAX(ListPrice), 0) AS MaxP, " +
                                $"COALESCE(MIN(ListPrice), 0) AS MinP, " +
                                $"COALESCE(MAX(Weight), 0) AS MaxW, " +
                                $"COALESCE(MIN(Weight), 0) AS MinW " +
                        $"FROM {fromTable}")
                    .FirstOrDefaultAsync();

                availableOptions.PriceAndWeight = MaxMin;
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
        public async Task<ActionResult<IEnumerable<GenericView>>> GetWithFiltersFromView([FromQuery] string view,
            ProductFilters Filters)
        {

            string ViewName = string.Empty;

            switch (view)
            {
                case "clothing":
                    ViewName = "[ClothingView]";
                    break;
            }

            string fromTable = $"[AdventureWorksLT2019].[SalesLT].{ViewName}";

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
                    if (pI is not null)  //ulteriore controllo di sicurezza (anche se da fEnd non dovrebbero arrivare dati compromessi)
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
                    if (wI is not null) //ulteriore controllo di sicurezza
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
                }
                if (weightConditions.Any())
                {
                    whereClauses.Add($"({string.Join(" OR ", weightConditions)})");
                }
            }

            var queryString = $"SELECT * FROM {fromTable} ";

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

            var query = _context.GenericViews
                .FromSql(sqlInterpolatedString);

            var filtered = await query.ToListAsync();
            return filtered;
        }
    }
}
