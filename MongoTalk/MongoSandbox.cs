using System;
using System.Linq;
using log4net;
using log4net.Config;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoTalk.Wrapper;

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

            Log.Info("Querying existing documents");
            FindNateAsBsonDocument(database);
            FindNateAsEmployeeObject(database);
            FindNateAsPersonObject(database);
            FindOldSoftwareEmployees(database);

            Log.Info("Inserting new documents");
            InsertBsonDocument(database);
            InsertSoftwareEmployees(database);
            UnsafelyInsertSoftwareEmployees(database);
            
            Log.Info("Querying existing documents using LINQ");
            FindOldSoftwareEmployeesWithLinq(database);
            FindIfThereAreAnyIntelligenceEmployees(database);

            Log.Info("Updating existing documents");
            UpdateNatesAgeToThirtySix(database);
            UpdateNatesPhoneNumber(database);

            Log.Info("Aggregating the employee information");
            AggregateOldEmployeesByDepartment(database);

            Log.Info("Test out our template");
            TryOutOurTemplate(database);

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
        
        private static void FindNateAsPersonObject(MongoDatabase database)
        {
            var domainCollection = database.GetCollection<Person>("employee");
            var employeeDocument = domainCollection.FindOne(Query.EQ("name", "Nate Buwalda"));
            Log.InfoFormat("Person document version is: {0}", employeeDocument);
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

        private static void FindOldSoftwareEmployeesWithLinq(MongoDatabase database)
        {
            var domainQueryable = database.GetCollection<Employee>("employee").AsQueryable();

            var employees = domainQueryable.Where(employee => 
                employee.Department == "Software Development"
                && employee.Age >= 30);

            var mongoQuery = ((MongoQueryable<Employee>) employees).GetMongoQuery();

            var json = mongoQuery.ToJson();
            Log.InfoFormat("The generated bson for the LINQ query was {0}", json);
            Log.InfoFormat("The query result was of type {0}", employees);

            foreach (var employee in employees)
            {
                Log.InfoFormat("{0} is an old software developer", employee.Name);
            }
        }

        private static void FindIfThereAreAnyIntelligenceEmployees(MongoDatabase database)
        {
            var domainQueryable = database.GetCollection<Employee>("employee").AsQueryable();

            var hasIntelligenceEmployees = domainQueryable.Any(employee => employee.Department == "Intelligence");

            Log.InfoFormat("There are intelligence employees? {0}", hasIntelligenceEmployees);
        }

        private static void InsertBsonDocument(MongoDatabase database)
        {
            Log.Info("Inserting a BsonDocument");
            var bsonCollection = database.GetCollection<BsonDocument>("employee");
            var bsonDocument = new BsonDocument
            {
                {"name", "Stan Smith"},
                {"department", "Intelligence"},
                {"age", 45}
            };
            
            var writeResult = bsonCollection.Insert(bsonDocument);
            Log.InfoFormat("The insert was successful? {0}", writeResult.Ok);

            var insertedDocument = bsonCollection.FindOne(Query.EQ("name", "Stan Smith"));
            Log.InfoFormat("The inserted bson document was {0}", insertedDocument);
        }

        private static void InsertSoftwareEmployees(MongoDatabase database)
        {
            Log.Info("Inserting a batch of employees");
            var domainCollection = database.GetCollection<Employee>("employee");
            var softwareEmployees = new[]
            {
                new Employee { Name = "Bill Gates", Department = "Software Development", Age = 58 },
                new Employee { Name = "Young Developer", Department = "Software Development", Age = 23 }
            };

            var writeResult = domainCollection.InsertBatch(softwareEmployees);
            Log.InfoFormat("The batch insert was successful? {0}", writeResult.All(wc => wc.Ok));

            FindOldSoftwareEmployees(database);
        }

        private static void UnsafelyInsertSoftwareEmployees(MongoDatabase database)
        {
            Log.Info("Inserting a batch of employees with a WriteConcern of 0");
            var domainCollection = database.GetCollection<Employee>("employee");
            var softwareEmployees = new[]
            {
                new Employee { Name = "Steve Ballmer", Department = "Software Development", Age = 58 },
                new Employee { Name = "Experienced Developer", Department = "Software Development", Age = 29 }
            };

            domainCollection.InsertBatch(softwareEmployees, WriteConcern.Unacknowledged);
            Log.Info("Not going to wait to inspect if the batch insert was successful.");

            FindOldSoftwareEmployees(database);
        }

        private static void UpdateNatesAgeToThirtySix(MongoDatabase database)
        {
            Log.Info("Making Nate older");
            var domainCollection = database.GetCollection<Employee>("employee");
            var query = Query.EQ("name", "Nate Buwalda");
            var update = Update.Set("age", 36);

            var writeResult = domainCollection.Update(query, update);
            Log.InfoFormat("The insert was successful? {0}", writeResult.Ok);

            FindNateAsEmployeeObject(database);
        }

        private static void UpdateNatesPhoneNumber(MongoDatabase database)
        {
            Log.Info("Adding a phone number to Nate");
            var domainCollection = database.GetCollection<Employee>("employee");
            var query = Query.EQ("name", "Nate Buwalda");
            var update = Update.Set("phoneNumber", "5151234567");

            var writeResult = domainCollection.Update(query, update);
            Log.InfoFormat("The insert was successful? {0}", writeResult.Ok);

            FindNateAsBsonDocument(database);
        }

        private static void AggregateOldEmployeesByDepartment(MongoDatabase database)
        {
            Log.Info("Getting a count of old employees by department");
            var domainCollection = database.GetCollection<Employee>("employee");
            
            var matchOn = new BsonDocument
            {
                {
                    "$match", new BsonDocument
                    {
                        {
                            "age", new BsonDocument {{"$gte", 30}}
                        }
                    }
                }
            };

            var groupBy = new BsonDocument
            {
                {
                    "$group", new BsonDocument
                    {
                        {
                            "_id", "$department"
                        }
                        ,{
                            "count", new BsonDocument
                            {
                                {"$sum", 1}
                            }
                        }
                    }
                }
            };

            var aggregateResult = domainCollection.Aggregate(matchOn, groupBy);
            Log.Info("The old employees per department were:");
            foreach (var resultDocument in aggregateResult.ResultDocuments)
            {
                Log.InfoFormat("Result - {0}", resultDocument);
            }
        }

        private static void TryOutOurTemplate(MongoDatabase database)
        {
            var template = new MongoTemplate<Employee>(database.GetCollection<Employee>("employee"));
            
            var nateFromTemplate = template.Read(Query.EQ("name", "Nate Buwalda")).FirstOrDefault();
            Log.InfoFormat("Template read results were {0}", nateFromTemplate);

            var newEmployee = new Employee {Name = "John Doe", Department = "S Mart", Age = 22};
            var insertedEmployee = template.Create(newEmployee);
            
            insertedEmployee.Age = 27;
            var updatedEmployee = template.Update(insertedEmployee);
            
            Log.InfoFormat("The result of all of our template operations was {0}", updatedEmployee);
        }

        private static void RemoveEveryoneExceptNate(MongoDatabase database)
        {
            var employeeCollection = database.GetCollection<Employee>("employee");
            employeeCollection.Remove(Query.NE("name", "Nate Buwalda"));

            var queryarbleCollection = employeeCollection.AsQueryable();
            var nateDocument = queryarbleCollection.First(employee => employee.Name == "Nate Buwalda");

            nateDocument.Age = 35;
            employeeCollection.Save(nateDocument);

            var documentsInCollection = queryarbleCollection.Count();
            Log.InfoFormat("There are {0} document(s) left in the employee collection after the removal", documentsInCollection);

            FindNateAsBsonDocument(database);
        }
    }
}
