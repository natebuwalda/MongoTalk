using JohnDeere.Common.Mongo.Connection;
using MongoDB.Driver;
using MongoTalk.Model;

namespace MongoTalk.Wrapper
{
    public class MongoDatabaseAdapter : IMongoDatabase
    {
        private readonly MongoDatabase _mongoDatabase;

        public MongoDatabaseAdapter(MongoDBConnectionInfo connectionInfo)
        {
            var mongoUrl = new MongoUrl(connectionInfo.DBUri);
            var settings = MongoClientSettings.FromUrl(mongoUrl);
            var credentials = MongoCredential.CreateMongoCRCredential(connectionInfo.DBName, connectionInfo.Username, connectionInfo.Password);
            settings.Credentials = new[] {credentials};

            var client = new MongoClient(settings);
            _mongoDatabase = client.GetServer().GetDatabase(connectionInfo.DBName);
        }

        public IMongoTemplate<T> GetCollection<T>(string collectionName) where T : Document
        {
            return new MongoTemplate<T>(_mongoDatabase.GetCollection<T>(collectionName));
        }

        public MongoDatabase GetDatabase()
        {
            return _mongoDatabase;
        }
    }
}