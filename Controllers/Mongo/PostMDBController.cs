using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Models.LTWorks;
using BetaCycle_Padova.Models.Mongo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BetaCycle_Padova.Controllers.Mongo
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostMDBController : ControllerBase
    {
        //FUNZIONA E NON TOCCARE MAI PIù

        private readonly AdventureWorksLt2019Context _LT2019Context;

        private IMongoCollection<BsonDocument> mongoBsonCollection;

        public PostMDBController(IOptions<PostMDBConfig> options, AdventureWorksLt2019Context Lt2019context)
        {
            var mongoClient = new MongoClient(options.Value.ConnectionString);
            var mongoDB = mongoClient.GetDatabase(options.Value.DatabaseName);
            mongoBsonCollection = mongoDB.GetCollection<BsonDocument>(options.Value.PostCollection);
            _LT2019Context = Lt2019context;
            
        }

        [HttpGet]
        public async Task<IEnumerable<object>> GetPostsMDB()
        {
            var list = new List<object>();

            var BsonDoc = await mongoBsonCollection.Find(obj => true).ToListAsync();

            foreach(var b in BsonDoc)
            {
                list.Add(b.ToJson());
            }

            return list;
        }

        [HttpGet("{p_id}")]
        public async Task<List<Post_Products>> GetPostMDB_ByProductID(int p_id)
        {
            //per swagger usare 680 e 788
                     
            var list = new List<Post_Products>();

            //filtro
            // var filter = Builders<BsonDocument>.Filter.Regex(p => p["productId"], new BsonRegularExpression($"{p_id}"));
            var filter = Builders<BsonDocument>.Filter.Eq("productId", p_id);

            var BsonDoc = await mongoBsonCollection.Find(filter).ToListAsync();
          

            foreach (var b in BsonDoc)
            {              
                Post_Products post_Products = new();

                //campi del SQL
                Product product = new();
                product = await _LT2019Context.Products.FirstOrDefaultAsync(p => p.ProductId == p_id);

                post_Products.ProductId = product.ProductId;             
                post_Products.ProductName = product.Name;
                post_Products.ProductColor = product.Color;
                post_Products.ProductListPrice = product.ListPrice;

                //campi del MongoDB
                post_Products.PostTitle = b["title"].AsString;
                post_Products.PostContent = b["content"].AsString;

                list.Add(post_Products);               
            }

            return list;
        }


    }
}
