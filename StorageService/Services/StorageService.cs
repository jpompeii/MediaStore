using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MediaStore.StorageService.Model;
using MediaStore.StorageService.Providers;

namespace MediaStore.StorageService.Services
{
    class StorageService : IStorageService
    {
        public Type DefaultStorageProvider { get; set; }

        private IServiceProvider _container;
        private ConcurrentDictionary<string, IStorageProvider> _services;

        public StorageService(IServiceProvider container)
        {
            _container = container;
            _services = new ConcurrentDictionary<string, IStorageProvider>();
            DefaultStorageProvider = Type.GetType("MediaStore.StorageService.Providers.LocalFS");
        }

        private IStorageProvider ResolveServiceAccount(string serviceAccount)
        {
            if (String.IsNullOrEmpty(serviceAccount))
                serviceAccount = GetCurrentUserAccount();

            IStorageProvider storageProvider = null;
            if (!_services.TryGetValue(serviceAccount, out storageProvider))
            {
                if (serviceAccount.StartsWith("x:"))
                {
                    // registered external account
                    var storageConfigId = serviceAccount.Substring(2);
                    // StorageServiceConfig cfg 
                    throw new NotImplementedException();
                }
                else
                {
                    storageProvider = _container.GetService(DefaultStorageProvider) as IStorageProvider;
                    _services[serviceAccount] = storageProvider;
                }
            }
            return storageProvider;
        }

        private string GetCurrentUserAccount()
        {
            // stubbed out for now.  This method must return the 
            // account identifier for the thread/request current user
            return "000000000001";
        }

        public string RegisterStorageServiceAccount(StorageServiceConfig serviceConfig)
        {
            /*
            if (_services.ContainsKey(serviceConfig.serviceAccount))
                throw new ArgumentException($"Attempting to register a storage service that exists already: {serviceConfig.serviceAccount}");

            Type svcType = Type.GetType(serviceConfig.serviceAccount, true);
            var provider = (IStorageProvider)_container.GetService(svcType);
            provider.Configure(serviceConfig);
            _services[serviceConfig.serviceAccount] = provider;
            */
            return "x:000000000000";
        }

        public void DeleteStorageServiceAccount(string accountId)
        {
        }

        public bool ContainsBucket(string bucketName, string serviceAccount)
        {
            IStorageProvider provider = ResolveServiceAccount(serviceAccount);
            return provider.ContainsBucket(bucketName);
        }

        public void CreateBucket(string bucketName, string serviceAccount)
        {
            IStorageProvider provider = ResolveServiceAccount(serviceAccount);
            provider.CreateBucket(bucketName);
        }

        public void DeleteBucket(string bucketName, string serviceAccount)
        {
            IStorageProvider provider = ResolveServiceAccount(serviceAccount);
            provider.CreateBucket(bucketName);
        }

        public Stream GetObjectContent(string bucketName, string objectName, string serviceAccount)
        {
            IStorageProvider provider = ResolveServiceAccount(serviceAccount);
            return provider.GetObjectContent(bucketName, objectName);
        }

        public bool IsServiceRegistered(string serviceAccount)
        {
            return _services.ContainsKey(serviceAccount);
        }

        public ICollection<Bucket> ListBuckets(string serviceAccount)
        {
            IStorageProvider provider = ResolveServiceAccount(serviceAccount);
            return provider.ListBuckets();
        }

        public void StoreObject(string bucketName, string objectName, Stream content, string serviceAccount)
        {
            IStorageProvider provider = ResolveServiceAccount(serviceAccount);
            provider.StoreObject(bucketName, objectName, content);
        }
    }
}
