using System;
using System.Linq;
using log4net;
using log4net.Config;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace MongoTalk
{
    public class MongoSandbox
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MongoSandbox));

        private const string MongoLocalhost = "mongodb://localhost:27017";
        private const string DatabaseName = "local-talk";
        private const string UserName = "local-mongo";
        private const string Password = "passw0rd";

        public static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            Log.Info("Starting Mongo Sandbox");

            var database = CreateConnection();

            Log.Info("Looking up existing documents");
            FindNateAsBsonDocument(database);
            FindNateAsEmployeeObject(database);
            FindOldSoftwareEmployees(database);

            Log.Info("Inserting new documents");
            InsertBsonDocument(database);

            Log.Info("Removing everyone except Nate");
            RemoveEveryoneExceptNate(database);

            Log.Info("Done running Mongo Sandbox");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static MongoDatabase CreateConnection()
        {
            Log.Info("Making connection to local-talk db");
            var mongoUrl = new MongoUrl(MongoLocalhost);
            var settings = MongoClientSettings.FromUrl(mongoUrl);
            var credentials = MongoCredential.CreateMongoCRCredential(DatabaseName, UserName, Password);
            settings.Credentials = new[] {credentials};

            var client = new MongoClient(settings);
            var database = client.GetServer().GetDatabase(DatabaseName);
            
            Log.Info("Connection established");
            return database;
        }

        private static void FindNateAsBsonDocument(MongoDatabase database)
        {
            var bsonCollection = database.GetCollection<BsonDocument>("employee");
            var bsonDocument = bsonCollection.FindOne(Query.EQ("name", "Nate Buwalda"));
            Log.InfoFormat("BsonDocument version is: {0}", bsonDocument);
        }

        private static void FindNateAsEmployeeObject(MongoDatabase database)
        {
            var domainCollection = database.GetCollection<Employee>("employee");
            var employeeDocument = domainCollection.FindOne(Query.EQ("name", "Nate Buwalda"));
            Log.InfoFormat("Employee document version is: {0}", employeeDocument);
        }

        private static void FindOldSoftwareEmployees(MongoDatabase database)
        {
            var domainCollection = database.GetCollection<Employee>("employee");
            var mongoQuery = Query.And(Query.EQ("department", "Software Development")
                                      , Query.GTE("age", 30));
            var employees = domainCollection.Find(mongoQuery);

            Log.InfoFormat("The query result was of type {0}", employees);

            foreach (var employee in employees)
            {
                Log.InfoFormat("{0} is an old software developer", employee.Name);
            }
        }

        private static void InsertBsonDocument(MongoDatabase database)
        {
            var bsonCollection = database.GetCollection<BsonDocument>("employee");
            var bsonDocument = new BsonDocument
            {
                {"name", "Stan Smith"},
                {"department", "Intelligence"},
                {"age", 45}
            };
            bsonCollection.Insert(bsonDocument);
            var insertedDocument = bsonCollection.FindOne(Query.EQ("name", "Stan Smith"));
            Log.InfoFormat("The inserted bson document was {0}", insertedDocument);
        }

        private static void RemoveEveryoneExceptNate(MongoDatabase database)
        {
            var employeeCollection = database.GetCollection<Employee>("employee");
            employeeCollection.Remove(Query.NE("name", "Nate Buwalda"));

            var documentsInCollection = employeeCollection.AsQueryable().Count();
            Log.InfoFormat("There were {0} document(s) left in the employee collection after the removal", documentsInCollection);
        }
    }
}
