using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoTalk.Model
{
    public abstract class Document
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("lmd")]
        public DateTime LastModifiedDate;
    }
}
