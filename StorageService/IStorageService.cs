using MediaStore.StorageService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MediaStore.StorageService
{
    public interface IObjectTransfer
    {
        Stream GetObjectContent(string storageId);
        void StoreObject(string storageId, Stream content);
    }

    public interface IStorageService
    {
        bool IsServiceRegistered(string serviceName);
        string RegisterStorageServiceAccount(StorageServiceConfig serviceConfig);
        void DeleteStorageServiceAccount(string accountId);
        void CreateBucket(string bucketName, string serviceAccount);  // container, acl, tags, region
        void DeleteBucket(string bucketName, string serviceAccount);
        bool ContainsBucket(string bucketName, string serviceAccount);
        ICollection<Bucket> ListBuckets(string serviceAccount);
        void StoreObject(string bucketName, string objectName, Stream content, string serviceAccount);
        Stream GetObjectContent(string bucketName, string objectName, string serviceAccount);
    }
}
