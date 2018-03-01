using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStore.StorageService.Model
{
    public class StorageObject
    {
        public string Id { get; private set; }
        public string BucketId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdate { get; set; }
        public long Size { get; set; }
        public string Directory { get; set; }

        public StorageObject()
        {
            DateCreated = LastUpdate = DateTime.UtcNow;
        }
    }
}
