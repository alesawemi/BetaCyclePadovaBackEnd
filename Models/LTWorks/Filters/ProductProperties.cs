using System.ComponentModel.DataAnnotations;

namespace BetaCycle_Padova.Models.LTWorks.Filters
{
    public class ProductProperties
    {
        public List<string> availableColors {  get; set; }
        public List<string> availableCategories { get; set; }
        public List<string> availableSizes { get; set; }       
        public PriceAndWeightMapping PriceAndWeight { get; set; }
        //public decimal priceMax { get; set; }
        //public decimal priceMin { get; set; }
        //public decimal weightMax { get; set; }
        //public decimal weightMin { get; set; }

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

    public class xProperty
    {
        [Key]
        public string? Option { get; set; }
        public xProperty (string option) { Option = option; }
        
    }

    public class PriceAndWeightMapping
    {
        [Key]
        public decimal MaxP { get; set; }
        public decimal MinP { get; set; }
        public decimal MaxW { get; set; }
        public decimal MinW { get; set; }

        public PriceAndWeightMapping()
        {
            MaxP = (30 * 100);
            MinP = 0;
            MaxW = (30 * 100);
            MinW = 0;
        }

        public PriceAndWeightMapping (decimal maxP, decimal minP, decimal maxW, decimal minW)
        {
            MaxP = maxP;
            MinP = minP;
            MaxW = maxW;
            MinW = minW;
        }
    }

}
