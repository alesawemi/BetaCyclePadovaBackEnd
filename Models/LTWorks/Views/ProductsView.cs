using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BetaCycle_Padova.Models.LTWorks.Views;

/// <summary>
/// Products sold or used in the manfacturing of sold products.
/// </summary>
public partial class ProductsView
{
    [Key]
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
    public decimal StandardCost { get; set; }
    public decimal ListPrice { get; set; }
    public string? Size { get; set; }
    public decimal? Weight { get; set; }
    //public int ProductCategoryId { get; set; }
    public string ProductCategory { get; set; }
    //public int ProductModelId { get; set; }
    public string ProductModel { get; set; }
    public byte[]? LargeImage { get; set; }
}
