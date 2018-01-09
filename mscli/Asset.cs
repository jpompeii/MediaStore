using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStore.Client
{
    public class Asset
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public int CurrentVersion { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdate { get; set; }

        public class EmbeddedClass
        {
            public Library Library;
        }
        public EmbeddedClass Embedded { get; set; }
    }
}
