using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BetaCycle_Padova.Models.LTWorks.Views
{
    /// <summary>
    /// Represents a view model for displaying clothing products.
    /// </summary>
    public partial class ClothingView
    {
        /// <summary>
        /// Gets or sets the unique identifier of the clothing product.
        /// </summary>
        [Key]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the name of the clothing product.
        /// </summary>
        public string ProductName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the color of the clothing product.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets the standard cost of the clothing product.
        /// </summary>
        public decimal StandardCost { get; set; }

        /// <summary>
        /// Gets or sets the list price of the clothing product.
        /// </summary>
        public decimal ListPrice { get; set; }

        /// <summary>
        /// Gets or sets the size of the clothing product.
        /// </summary>
        public string? Size { get; set; }

        /// <summary>
        /// Gets or sets the weight of the clothing product.
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// Gets or sets the category of the clothing product.
        /// </summary>
        public string ProductCategory { get; set; } = null!;
    }
}
