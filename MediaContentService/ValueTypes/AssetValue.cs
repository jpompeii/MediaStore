
using MediaContentService.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaContentService.ValueTypes
{
    public class AssetValue
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public int CurrentVersion { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdate { get; set; }

        public class EmbeddedClass
        {
            public LibraryValue Library;
        }
        public EmbeddedClass Embedded;

        public static AssetValue FromModel(Asset asset)
        {
            return new AssetValue
            {
                Id = asset.Id,
                Name = asset.Name,
                DateCreated = asset.DateCreated,
                LastUpdate = asset.LastUpdate,
                Embedded = new EmbeddedClass
                {
                    Library = LibraryValue.FromModel(asset.Library)
                }
            };
        }

        public static IList<AssetValue> FromModel(IEnumerable<Asset> modelLibs)
        {
            return modelLibs.Select(l => FromModel(l)).ToList();
        }
    }
}
