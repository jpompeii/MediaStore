using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaContentService.Model
{
    public class MediaStoreContext
    {
        public IMongoDatabase Database { get; }
        private static string _lock = "lock";

        private static IDictionary<string, string> typedCollections = new Dictionary<string, string>();

        public MediaStoreContext()
        {
            var client = new MongoClient("mongodb://localhost");
            Database = client.GetDatabase("mediastore");
        }

        public static MediaStoreContext GetContext()
        {
            MediaStoreContext ctx;
            lock (_lock)
            {
                ctx = ContextThreadData.Current;
                if (ctx == null)
                {
                    ctx = new MediaStoreContext();
                    ContextThreadData.Current = ctx;
                }
            }
            return ctx;
        }

        public static void FreeContext()
        {
            ContextThreadData.Current = null;
        }

        public IMongoCollection<Library> Libraries
        {
            get
            {
                return Database.GetCollection<Library>(typedCollections[typeof(Library).Name]);
            }
        }

        public Library FindLibrary(string id)
        {
            var filter = new BsonDocument
            {
                { "LibraryName", id }
            };

            // var filter = $"{{ LibraryName: \"{id}\"}}";
            return Libraries.Find(filter).FirstOrDefault();
        }

        public IMongoCollection<Account> Accounts
        {
            get
            {
                return Database.GetCollection<Account>(typedCollections[typeof(Account).Name]);
            }
        }

        public Account FindAccount(string id)
        {
            var filter = new BsonDocument
            {
                { "Name", id }
            };
            return Accounts.Find(filter).FirstOrDefault();
        }

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

        public IList<Asset> GetAssets(Library lib, string start, int count)
        {
            var cltn = Database.GetCollection<Asset>(lib.AssetCollection);
            var filter = new BsonDocument
            {
                { "LibraryId", lib.Id }
            };
            return cltn.Find(filter).ToList();
        }

        public static void ConfigureModel()
        {
            BsonClassMap.RegisterClassMap<Library>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
                cm.MapProperty(c => c.AccountId);
                cm.UnmapProperty(c => c.Account);
            });

            BsonClassMap.RegisterClassMap<Account>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<Asset>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
                cm.MapExtraElementsMember(c => c.MetadataElements);
                cm.UnmapProperty(c => c.Library);
            });

            typedCollections[typeof(Library).Name] = "libraries";
            typedCollections[typeof(Account).Name] = "accounts";
        }
    }

    public class ContextThreadData
    {
        [ThreadStatic]
        public static MediaStoreContext Current;

        public ContextThreadData()
        {
            Current = null;
        }
    }
}
