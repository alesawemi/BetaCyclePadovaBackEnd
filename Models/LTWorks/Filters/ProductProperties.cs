namespace BetaCycle_Padova.Models.LTWorks.Filters
{
    public class ProductProperties
    {
        public List<string> availableColors {  get; set; }
        public List<string> availableCategories { get; set; }
        public decimal priceMax { get; set; }
        public decimal priceMin { get; set; }
        public decimal weightMax { get; set; }
        public decimal weightMin { get; set; }
        public string sizeMax { get; set; }
        public string sizeMin { get; set; } 

        public ProductProperties()
        {
            availableColors = new List<string> { };
            availableCategories = new List<string> { };
            priceMax = 0;
            priceMin = 30000;
            weightMax = 0;
            weightMin = 30000;
            sizeMax = string.Empty;
            sizeMin = string.Empty;
        }
    }

    public class xProperty
    {
        public string? Option { get; set; }

        public xProperty (string option)
        { Option = option; }
    }
}
