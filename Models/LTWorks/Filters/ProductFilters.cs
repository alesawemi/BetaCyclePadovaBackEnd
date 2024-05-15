namespace BetaCycle_Padova.Models.LTWorks.Filters
{
    public class ProductFilters
    {
        public string productName { get; set; }
        public string? color { get; set; }
        public string? size { get; set; }
        public string productCategory { get; set; }
        public decimal? maxPrice { get; set; }
        public decimal? minPrice { get; set; }
        public decimal? maxWeight { get; set; }
        public decimal? minWeight { get; set; }
        public bool descPrice { get; set; }
        public bool ascPrice { get; set; }

        public ProductFilters() 
        {
            productName = "allProducts";
            color = "color";
            size = "size";
            productCategory = "category";
            maxPrice = 0;
            minPrice = 0;
            maxWeight = 0;
            minWeight = 0;
            descPrice = false;
            ascPrice = false;   
        }
    }
}
