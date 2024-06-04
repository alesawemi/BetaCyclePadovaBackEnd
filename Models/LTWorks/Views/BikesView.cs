using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BetaCycle_Padova.Models.LTWorks.Views
{
    /// <summary>
    /// Represents a view model for displaying bikes.
    /// </summary>
    public partial class BikesView
    {
        /// <summary>
        /// Gets or sets the unique identifier of the bike product.
        /// </summary>
        [Key]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the name of the bike product.
        /// </summary>
        public string ProductName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the color of the bike.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets the standard cost of the bike.
        /// </summary>
        public decimal StandardCost { get; set; }

        /// <summary>
        /// Gets or sets the list price of the bike.
        /// </summary>
        public decimal ListPrice { get; set; }

        /// <summary>
        /// Gets or sets the size of the bike.
        /// </summary>
        public string? Size { get; set; }

        /// <summary>
        /// Gets or sets the weight of the bike.
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// Gets or sets the category of the bike product.
        /// </summary>
        public string ProductCategory { get; set; } = null!;
    }
}
