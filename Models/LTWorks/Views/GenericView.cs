﻿using System.ComponentModel.DataAnnotations;

namespace BetaCycle_Padova.Models.LTWorks.Views
{
    public class GenericView
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Color { get; set; }
        public decimal StandardCost { get; set; }
        public decimal ListPrice { get; set; }
        public string? Size { get; set; }
        public decimal? Weight { get; set; }
        public string ProductCategory { get; set; } 
        public string ProductModel { get; set; }
        public byte[]? LargeImage { get; set; }
    }
}
