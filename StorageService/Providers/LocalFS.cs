using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MediaStore.StorageService.Model;

namespace MediaStore.StorageService.Providers
{
    public class LocalFS : IStorageProvider
    {
        private string _fsRoot;
        private LocalFSContext _dataContext;
        private Dictionary<string, string> _bucketCache;
        private string _lockObj = "lock";

        public LocalFS(LocalFSContext context)
        {
            _dataContext = context;

        }

        public void Configure(StorageServiceConfig serviceConfig)
        {
            throw new NotImplementedException();
        }

        private Bucket getBucket(string bucketName, bool throwOnError)
        {
            lock (_lockObj)
            {
                if (_bucketCache.ContainsKey(bucketName))
                    return new Bucket(_bucketCache[bucketName], bucketName);

                Bucket bucket = _dataContext.FindBucket(bucketName);
                if (bucket != null)
                    _bucketCache[bucketName] = bucket.Id;
                else if (throwOnError)
                    throw new ArgumentException($"Bucket with name {bucketName} does not exist");

                return bucket;
            }
        }

        public void CreateBucket(string bucketName)
        {
            if (String.IsNullOrEmpty(bucketName))
            {
                // invalid bucket name
                throw new ArgumentNullException("Bucket name is either null or empty");
            }

            var bucketRoot = Path.Combine(_fsRoot, bucketName);
            var bucket = _dataContext.FindBucket(bucketName);
            if (bucket != null || Directory.Exists(bucketRoot))
            {
                // bucket exists already
                throw new ArgumentException($"Bucket wtih name {bucketName} exists already");
            }

            try { Directory.CreateDirectory(bucketRoot); }
            catch (Exception ex)
            {
                throw new ArgumentException($"Bucket wtih name \"{ bucketName }\" is invalid", ex);
            }

            bucket = new Bucket
            {
                Name = bucketName
            };
            try {
                _dataContext.Buckets.InsertOne(bucket);
                lock(_lockObj)
                    _bucketCache[bucketName] = bucket.Id;
            }
            catch (Exception ex)
            {
                Directory.Delete(bucketRoot);
                throw ex;
            }
        }

        public void DeleteBucket(string bucketName)
        {
            throw new NotImplementedException();
        }

        public bool ContainsBucket(string bucketName)
        {
            return getBucket(bucketName, false) != null;
        }

        public ICollection<Bucket> ListBuckets()
        {
            return _dataContext.GetBuckets();
        }

        public Stream GetObjectContent(string bucketName, string objectName)
        {
            Bucket bucket = getBucket(bucketName, true);
            StorageObject obj = _dataContext.FindObject(bucket, objectName);
            if (obj == null)
            {
                throw new ArgumentException($"Object with name {objectName} does not exist");
            }
            string path = Path.Combine(_fsRoot, bucket.Name, obj.Id);
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        public void StoreObject(string bucketName, string objectName, Stream content)
        {
            Bucket bucket = getBucket(bucketName, true);
            StorageObject obj = _dataContext.FindObject(bucket, objectName);
            bool newFile = false;
            if (obj == null)
            {
                obj = new StorageObject
                {
                    BucketId = bucket.Id,
                    Name = objectName,
                    DateCreated = DateTime.UtcNow,
                    LastUpdate = DateTime.UtcNow,
                    Size = content.Length,
                    Directory = GetDirectoryName(DateTime.UtcNow)
                };
                _dataContext.StorageObjects.InsertOne(obj);
                newFile = true;
            }
            string path = Path.Combine(_fsRoot, bucket.Name, obj.Directory);
            string tempPath = Path.Combine(path, Path.GetRandomFileName() + ".tmp");
            string objPath = Path.Combine(path, obj.Id);
            string tempObjPath = objPath + ".tmp";
            try
            {
                using (var outStr = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    content.CopyTo(outStr);
                }
                if (!newFile)
                    File.Move(objPath, tempObjPath);
                
                File.Move(tempPath, objPath);
                if (!newFile)
                    File.Delete(tempObjPath);
            }
            catch (Exception ex)
            {
                // make an attempt to undo changes
                try
                {
                    if (newFile)
                        _dataContext.DeleteObject<StorageObject>(obj.Id);

                    if (File.Exists(tempObjPath))
                        File.Move(tempObjPath, objPath);

                    File.Delete(tempPath);
                }
                catch (Exception) { }
                throw ex;
            }
        }

        private string GetDirectoryName(DateTime date)
        {
            TimeSpan span = DateTime.Now.Subtract(new DateTime(2018, 1, 1, 0, 0, 0));
            return $"{(int)span.TotalDays}";
        }

        
    }
}
