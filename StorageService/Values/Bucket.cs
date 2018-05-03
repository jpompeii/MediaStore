using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaStore.StorageService.Values
{
    public class Bucket
    {
        public string Name { get; set; }

        public Bucket()
        {
        }

        public static Bucket FromModel(MediaStore.StorageService.Model.Bucket bucket)
        {
            return new Bucket { Name = bucket.Name };
        }

        public static ICollection<Bucket> FromModel(ICollection<MediaStore.StorageService.Model.Bucket> buckets)
        {
            return buckets.Select(b => FromModel(b)).ToList();
        }
        public static MediaStore.StorageService.Model.Bucket ToModel(Bucket bucket)
        {
            return new MediaStore.StorageService.Model.Bucket { Name = bucket.Name };
        }

    }
}
