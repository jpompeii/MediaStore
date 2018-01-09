using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace MediaStore.Client
{
    public class MediaStoreClient
    {
        private string _baseUrl;
        private HttpClient _client;

        // usage: mscli baseAddr library file

        public MediaStoreClient(string baseUrl)
        {
            _baseUrl = baseUrl;
            _client = new HttpClient();
            _client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<Library> GetLibraryAsync(string libName)
        {
            var result = await _client.GetAsync("api/mediastore/libraries");
            var jsonString = result.Content.ReadAsStringAsync().Result;
            var json = result.Content.ReadAsStreamAsync().Result;
            var serializer = new DataContractJsonSerializer(typeof(List<Library>));
            var libraries = serializer.ReadObject(json) as List<Library>;
            return libraries.Where(c => c.LibraryName == libName).FirstOrDefault();
        }

        public async Task<Asset> AddFileAsync(string filePath, string libraryId)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            ByteArrayContent bytes = new ByteArrayContent(fileBytes);
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            multiContent.Add(bytes, "file", Path.GetFileName(filePath));
            var result = await _client.PostAsync($"api/mediastore/libraries/{libraryId}/asset", multiContent);
            var json = result.Content.ReadAsStreamAsync().Result;
            var serializer = new DataContractJsonSerializer(typeof(Asset));
            return serializer.ReadObject(json) as Asset;
        }
    }
}
