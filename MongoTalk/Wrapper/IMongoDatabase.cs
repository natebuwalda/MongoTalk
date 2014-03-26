using MongoTalk.Model;

namespace MongoTalk.Wrapper
{
    public interface IMongoDatabase
    {
        IMongoTemplate<T> GetCollection<T>(string collectionName) where T : Document;
    }
}
