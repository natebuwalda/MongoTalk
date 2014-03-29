using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
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

        public IEnumerable<T> Read(IMongoQuery query)
        {
            return _mongoCollection.Find(query);
        }

        public T Create(T entity)
        {
            var writeResult = _mongoCollection.Insert(entity);
            if (writeResult.Ok)
            {
                return entity;
            }
            else
            {
                throw new Exception("Unable to insert into collection");
            }
        }

        public T Update(T entity)
        {
            var writeResult = _mongoCollection.Save(entity);
            if (writeResult.Ok)
            {
                return entity;
            }
            else
            {
                throw new Exception("Unable to insert into collection");
            }
        }

        public bool Remove(T entity)
        {
            var writeResult = _mongoCollection.Remove(Query.EQ("_id", entity.Id));
            return writeResult.Ok;
        }
    }
}
