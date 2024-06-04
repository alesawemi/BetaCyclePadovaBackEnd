using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BetaCycle_Padova.Models.LTWorks.Views
{
    /// <summary>
    /// Represents a view model for displaying products sold or used in the manufacturing of sold products.
    /// </summary>
    public partial class ProductsView
    {
        /// <summary>
        /// Gets or sets the unique identifier of the product.
        /// </summary>
        [Key]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string ProductName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the color of the product.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets the standard cost of the product.
        /// </summary>
        public decimal StandardCost { get; set; }

        /// <summary>
        /// Gets or sets the list price of the product.
        /// </summary>
        public decimal ListPrice { get; set; }

        /// <summary>
        /// Gets or sets the size of the product.
        /// </summary>
        public string? Size { get; set; }

        /// <summary>
        /// Gets or sets the weight of the product.
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// Gets or sets the category of the product.
        /// </summary>
        public string ProductCategory { get; set; }

        /// <summary>
        /// Gets or sets the model of the product.
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// Gets or sets the large image of the product.
        /// </summary>
        public byte[]? LargeImage { get; set; }

        /// <summary>
        /// Gets or sets the main category ID of the product.
        /// </summary>
        public int MainCategoryID { get; set; }
    }
}
