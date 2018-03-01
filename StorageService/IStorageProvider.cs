using MediaStore.StorageService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MediaStore.StorageService
{
    public interface IStorageProvider
    {
        void Configure(StorageServiceConfig serviceConfig);
        void CreateBucket(string bucketName);  // container, acl, tags, region
        void DeleteBucket(string bucketName);
        ICollection<Bucket> ListBuckets();
        bool ContainsBucket(string bucketName);
        void StoreObject(string bucketName, string objectName, Stream content);
        Stream GetObjectContent(string bucketName, string objectName);
    }
}
