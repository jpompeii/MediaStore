using MediaStore.StorageService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MediaStore.StorageService
{
    public interface IStorageService
    {
        void CreateBucket(string bucketName);  // container, acl, tags, region
        ICollection<Bucket> ListBuckets();
        void StoreObject(string bucketName, string objectName, Stream content);
        Stream GetObjectContent(string bucketName, string objectName);
    }
}
