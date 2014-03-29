using MongoDB.Bson.Serialization.Attributes;
using MongoTalk.Model;

namespace MongoTalk
{
    /*
     BsonDocument version of Employee looks like this:
         {
            "_id" : ObjectId("533629ab3734625bd0d701ae"),
            "name" : "Nate Buwalda",
            "department" : "Software Development",
            "age" : 35
         }
     */

    [BsonIgnoreExtraElements]
    public class Employee : Document
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("department")]
        public string Department { get; set; }

        [BsonElement("age"), BsonIgnoreIfDefault]
        public int Age { get; set; }

        public override string ToString()
        {
            return string.Format("Employee - Name: {0}, Department: {1}, Age: {2}", Name, Department, Age);
        }
    }

    [BsonIgnoreExtraElements]
    public class Person : Document
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("age"), BsonIgnoreIfDefault]
        public int Age { get; set; }

        public override string ToString()
        {
            return string.Format("Person - Name: {0}, Age: {1}", Name, Age);
        }
    }
}