using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BetaCycle_Padova.Models.LTWorks.OptionsAndFilters
{
    /// <summary>
    /// Represents properties related to products, such as available colors, categories, and sizes.
    /// </summary>
    public class ProductProperties
    {
        /// <summary>
        /// Gets or sets the list of available colors for products.
        /// </summary>
        public List<string?> availableColors { get; set; }

        /// <summary>
        /// Gets or sets the list of available categories for products.
        /// </summary>
        public List<string?> availableCategories { get; set; }

        /// <summary>
        /// Gets or sets the list of available sizes for products.
        /// </summary>
        public List<string?> availableSizes { get; set; }

        /// <summary>
        /// Gets or sets the mapping of price and weight ranges for products.
        /// </summary>
        public PriceAndWeightMapping PriceAndWeight { get; set; }

        // Additional commented out properties
        //public decimal priceMax { get; set; }
        //public decimal priceMin { get; set; }
        //public decimal weightMax { get; set; }
        //public decimal weightMin { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductProperties"/> class.
        /// </summary>
        public ProductProperties()
        {
            availableColors = new List<string> { };
            availableCategories = new List<string> { };
            availableSizes = new List<string> { };
            PriceAndWeight = new PriceAndWeightMapping();
            //priceMax = 30000;
            //priceMin = 0;
            //weightMax = 30000;
            //weightMin = 0;
        }
    }

    /// <summary>
    /// Represents an individual property option for products.
    /// </summary>
    public class xProperty
    {
        [Key]
        /// <summary>
        /// Gets or sets the option value.
        /// </summary>
        public string? Option { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="xProperty"/> class.
        /// </summary>
        /// <param name="option">The value of the option.</param>
        public xProperty(string? option) { Option = option; }
    }

    /// <summary>
    /// Represents a mapping of price and weight ranges for products.
    /// </summary>
    public class PriceAndWeightMapping
    {
        [Key]
        /// <summary>
        /// Gets or sets the maximum price for products.
        /// </summary>
        public decimal MaxP { get; set; }

        /// <summary>
        /// Gets or sets the minimum price for products.
        /// </summary>
        public decimal MinP { get; set; }

        /// <summary>
        /// Gets or sets the maximum weight for products.
        /// </summary>
        public decimal MaxW { get; set; }

        /// <summary>
        /// Gets or sets the minimum weight for products.
        /// </summary>
        public decimal MinW { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceAndWeightMapping"/> class.
        /// </summary>
        public PriceAndWeightMapping()
        {
            MaxP = 0;
            MinP = 0;
            MaxW = 0;
            MinW = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceAndWeightMapping"/> class with specified price and weight ranges.
        /// </summary>
        /// <param name="maxP">The maximum price.</param>
        /// <param name="minP">The minimum price.</param>
        /// <param name="maxW">The maximum weight.</param>
        /// <param name="minW">The minimum weight.</param>
        public PriceAndWeightMapping(decimal maxP, decimal minP, decimal maxW, decimal minW)
        {
            MaxP = maxP;
            MinP = minP;
            MaxW = maxW;
            MinW = minW;
        }
    }
}
