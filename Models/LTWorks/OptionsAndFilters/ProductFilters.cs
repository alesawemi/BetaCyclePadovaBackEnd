namespace BetaCycle_Padova.Models.LTWorks.OptionsAndFilters
{
    /// <summary>
    /// Represents the filters and options for product search.
    /// </summary>
    public class ProductFilters
    {
        /// <summary>
        /// Gets or sets a value indicating whether to sort products by descending price.
        /// </summary>
        public bool descPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sort products by ascending price.
        /// </summary>
        public bool ascPrice { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string productName { get; set; }

        /// <summary>
        /// Gets or sets the color of the product.
        /// </summary>
        public string? color { get; set; }

        /// <summary>
        /// Gets or sets the size of the product.
        /// </summary>
        public string? size { get; set; }

        /// <summary>
        /// Gets or sets the category of the product.
        /// </summary>
        public string productCategory { get; set; }

        /// <summary>
        /// Gets or sets the maximum price filter for the product.
        /// </summary>
        public decimal? maxPrice { get; set; }

        /// <summary>
        /// Gets or sets the minimum price filter for the product.
        /// </summary>
        public decimal? minPrice { get; set; }

        /// <summary>
        /// Gets or sets the maximum weight filter for the product.
        /// </summary>
        public decimal? maxWeight { get; set; }

        /// <summary>
        /// Gets or sets the minimum weight filter for the product.
        /// </summary>
        public decimal? minWeight { get; set; }

        /// <summary>
        /// Gets or sets the list of price intervals for filtering.
        /// </summary>
        public List<Interval> pIntervals { get; set; } = new List<Interval>();

        /// <summary>
        /// Gets or sets the list of weight intervals for filtering.
        /// </summary>
        public List<Interval> wIntervals { get; set; } = new List<Interval>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilters"/> class.
        /// </summary>
        public ProductFilters()
        {
            descPrice = false;
            ascPrice = false;
            productName = string.Empty;
            color = string.Empty;
            size = string.Empty;
            productCategory = string.Empty;
            maxPrice = 0;
            minPrice = 0;
            maxWeight = 0;
            minWeight = 0;
            pIntervals = new List<Interval>();
            wIntervals = new List<Interval>();
        }
    }

    /// <summary>
    /// Represents an interval with a minimum and maximum value.
    /// </summary>
    public class Interval
    {
        /// <summary>
        /// Gets or sets the minimum value of the interval.
        /// </summary>
        public decimal min { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum value of the interval.
        /// </summary>
        public decimal max { get; set; } = 0;
    }
}
