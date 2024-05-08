using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BetaCycle_Padova.Models.LTWorks;

public partial class ClothingView
{
    [Key]
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Color { get; set; }

    public decimal StandardCost { get; set; }

    public decimal ListPrice { get; set; }

    public string? Size { get; set; }

    public decimal? Weight { get; set; }

    public string ProductCategory { get; set; } = null!;
}
