using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaContentService.Model
{
    public class Asset
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public int CurrentVersion { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdate { get; set; }
        public string LibraryId { get; set; }

        public IList<AssetFile> AssetFiles { get; set; }
        public string AssetCollection { get; set; }
        public BsonDocument MetadataElements { get; set; }

        private Library _library;

        public Asset()
        {
            DateCreated = LastUpdate = DateTime.UtcNow;
            AssetFiles = new List<AssetFile>();
            MetadataElements = new BsonDocument();
        }

        public Library Library
        {
            get
            {
                if (_library == null && LibraryId != null)
                {
                    _library = MediaStoreContext.GetContext().FindObjectById<Library>(LibraryId);
                }
                return _library;
            }
            set
            {
                _library = value;
                LibraryId = _library.Id;
            }
        }

        public async Task AddToCollection(string cltnName)
        {
            AssetCollection = cltnName;
            IMongoDatabase db = MediaStoreContext.GetContext().Database;
            var cltn = db.GetCollection<Asset>(cltnName);
            await cltn.InsertOneAsync(this);
        }


        /*
    DataSets: {
        _id:
        keyName:
        isShared:
        fields: [
            {
                fieldName:
                fieldType:
                valueSet:
                flags:
                defaultValue

            }
        ]


    Asset: {
        _id: "xxx",
        filePath: "12/2/4/ikdiiekeu-2"
        name: "winterscene.jpg"
        dateUploaded: 
        currentVersion: 2
        pastVersions: [
            { versionId: "1", filePath: "12/2/4/ikdiiekeu-1", fileType: "Image", mimeType: "" }                
        ]

        tags: [
            ".ABCDE.FGHIDK."
        ]

// keyword objects:

        ds_imageMetadata: {
            camera: "Pentax K200D"
            lens: "18-200mm zoom"
            exposure: "006sec"
            dateTime: 
        }

        ds_project: "HJSHHHSKKSH"

        location: {
            city: "Mentor"
            state: "OH"

        }

    }

*/
    }
}
