using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaStore.StorageService.Model
{
    public class LocalFSContext
    {
        public IMongoDatabase Database { get; }
        private static IDictionary<string, string> typedCollections = new Dictionary<string, string>();

        public LocalFSContext()
        {
            var client = new MongoClient("mongodb://localhost");
            Database = client.GetDatabase("localfs");
        }

        public IMongoCollection<Bucket> Buckets
        {
            get
            {
                return Database.GetCollection<Bucket>(typedCollections[typeof(Bucket).Name]);
            }
        }

        public Bucket FindBucket(string name)
        {
            var filter = new BsonDocument
            {
                { "Name", name }
            };
            return Buckets.Find(filter).FirstOrDefault();
        }

        public ICollection<Bucket> GetBuckets()
        {
            var filter = new BsonDocument();
            return Buckets.Find(filter).ToList();
        }

        public IMongoCollection<StorageObject> StorageObjects
        {
            get
            {
                return Database.GetCollection<StorageObject>(typedCollections[typeof(StorageObject).Name]);
            }
        }

        public StorageObject FindObject(Bucket bucket, string objectName)
        {
            var filter = new BsonDocument
            {
                { "Name", objectName },
                { "BucketId", bucket.Id }
            };
            return StorageObjects.Find(filter).FirstOrDefault();
        }

        /*
         * MongoDB common operations
         * Todo: move these to a base class
         */

        public T FindObjectById<T>(string objectId)
        {
            string cltnName = typedCollections[typeof(T).Name];
            var cltn = Database.GetCollection<T>(cltnName);
            var filter = new BsonDocument
            {
                { "_id", objectId }
            };
            return cltn.Find(filter).FirstOrDefault();
        }

        public T FindObjectById<T>(string cltnName, string objectId)
        {
            var cltn = Database.GetCollection<T>(cltnName);
            var filter = new BsonDocument
            {
                { "_id", objectId }
            };
            return cltn.Find(filter).FirstOrDefault();
        }

        public IList<T> GetOneToMany<T>(string relationProp, string id)
        {
            string cltnName = typedCollections[typeof(T).Name];
            var cltn = Database.GetCollection<T>(cltnName);
            var filter = new BsonDocument
            {
                { relationProp, id }
            };
            return cltn.Find(filter).ToList();
        }

        public long DeleteObject<T>(string id)
        {
            string cltnName = typedCollections[typeof(T).Name];
            var cltn = Database.GetCollection<T>(cltnName);
            var filter = new BsonDocument
            {
                { "_id", id }
            };
            return cltn.DeleteOne(filter).DeletedCount;
        }

        public static void ConfigureModel()
        {
            BsonClassMap.RegisterClassMap<Bucket>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<StorageObject>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
            });

            typedCollections[typeof(Bucket).Name] = "buckets";
            typedCollections[typeof(StorageObject).Name] = "objects";
        }


    }
}
