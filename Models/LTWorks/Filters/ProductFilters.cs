namespace BetaCycle_Padova.Models.LTWorks.Filters
{
    public class ProductFilters
    {
        public bool descPrice { get; set; }
        public bool ascPrice { get; set; }
        public string productName { get; set; }
        public string? color { get; set; }
        public string? size { get; set; }
        public string productCategory { get; set; }
        public decimal? maxPrice { get; set; }
        public decimal? minPrice { get; set; }
        public decimal? maxWeight { get; set; }
        public decimal? minWeight { get; set; }
        public List<Interval> pIntervals { get; set; } 
        public List<Interval> wIntervals { get; set; }

        public ProductFilters()
        {
            descPrice = false;
            ascPrice = false;
            productName = "allProducts";
            color = "color";
            size = "size";
            productCategory = "category";
            maxPrice = 0;
            minPrice = 0;
            maxWeight = 0;
            minWeight = 0;  
            pIntervals = new List<Interval>();
            wIntervals = new List<Interval>();
        }
    }

    public class Interval
    {
        public decimal min { get; set; }
        public decimal max { get; set; }
    }


}
