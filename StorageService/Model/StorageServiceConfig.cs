using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStore.StorageService.Model
{
    public class StorageServiceConfig
    {
        public string ServiceProvider { get; set; }
        public string ServiceUrl { get; set; }
        public string Region { get; set; }
        public string AccountId { get; set; }
        public string AccountSecret { get; set; }
    }
}
