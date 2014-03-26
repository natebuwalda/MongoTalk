using System;
using System.Collections.Specialized;

namespace JohnDeere.Common.Mongo.Connection
{
    public class MongoDBConnectionInfo
    {
        public string DBUri { get; set; }
        public string DBName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public TimeSpan ConnectionTimeout { get; set; }
        public TimeSpan SocketTimeout { get; set; }
        public int WriteRetries { get; set; }
        public int WriteRetriesWait { get; set; }

        public static MongoDBConnectionInfo CreateMongoDbFromConfig(NameValueCollection appSettings)
        {
            var mongoInfo = new MongoDBConnectionInfo
                {
                    DBUri = appSettings["MongoDBUri"],
                    DBName = appSettings["MongoDBName"],
                    Username = appSettings["MongoDBUsername"],
                    Password = appSettings["MongoDBPassword"],
                    ConnectionTimeout = TimeSpan.FromSeconds(Convert.ToDouble(appSettings["MongoDBConnectionTimeout"])),
                    SocketTimeout = TimeSpan.FromSeconds(Convert.ToDouble(appSettings["MongoDBSocketTimeout"])),
                    WriteRetries = appSettings["MongoDBWriteRetries"] == null ? 4 : Convert.ToInt32(appSettings["MongoDBWriteRetries"]),
                    WriteRetriesWait = appSettings["MongoDBWriteRetriesWait"] == null ? 2 : Convert.ToInt32(appSettings["MongoDBWriteRetriesWait"])
                };
            return mongoInfo;
        }
    }
}
