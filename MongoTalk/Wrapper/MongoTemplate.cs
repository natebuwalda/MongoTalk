using MongoDB.Driver;
using MongoTalk.Model;

namespace MongoTalk.Wrapper
{
    public class MongoTemplate<T> : IMongoTemplate<T> where T : Document
    {
        private readonly MongoCollection<T> _mongoCollection;

        public MongoTemplate(MongoCollection<T> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

    }
}
