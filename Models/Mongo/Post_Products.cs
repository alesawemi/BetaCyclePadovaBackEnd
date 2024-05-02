using MongoDB.Bson.Serialization.Attributes;

namespace BetaCycle_Padova.Models.Mongo
{
    public class Post_Products
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? ProductColor { get; set; }

        public decimal ProductListPrice { get; set; }

        public string? PostTitle {  get; set; }

        public string? PostContent {  get; set; }

    }
}
