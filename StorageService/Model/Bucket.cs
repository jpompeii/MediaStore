using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStore.StorageService.Model
{
    public class Bucket
    {
        public string Id { get; private set; }
        public string AccountId { get; set; }
        public string Name { get; set; }

        public Bucket()
        {
        }

        public Bucket(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
