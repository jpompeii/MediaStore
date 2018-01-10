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

        public MediaStoreClient(string baseUrl)
        {
            _baseUrl = baseUrl;
            _client = new HttpClient();
            _client.BaseAddress = new Uri(baseUrl);
        }

        public async Task<Library> GetLibraryAsync(string libName)
        {
            var result = await _client.GetAsync("api/mediastore/libraries");
            if (result.IsSuccessStatusCode)
            {
                var json = result.Content.ReadAsStreamAsync().Result;
                var serializer = new DataContractJsonSerializer(typeof(List<Library>));
                var libraries = serializer.ReadObject(json) as List<Library>;
                return libraries.Where(c => c.LibraryName == libName).FirstOrDefault();
            }
            else
                throw new ApplicationException($"API Error: {result.StatusCode}");
        }

        public async Task<Asset> AddFileAsync(string filePath, string libraryId)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            ByteArrayContent bytes = new ByteArrayContent(fileBytes);
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            multiContent.Add(bytes, "file", Path.GetFileName(filePath));
            var result = await _client.PostAsync($"api/mediastore/libraries/{libraryId}/assets", multiContent);
            if (result.IsSuccessStatusCode)
            {
                var jsonString = result.Content.ReadAsStringAsync().Result;
                var json = result.Content.ReadAsStreamAsync().Result;
                var serializer = new DataContractJsonSerializer(typeof(List<Asset>));
                IList<Asset> assets = serializer.ReadObject(json) as List<Asset>;
                return assets.FirstOrDefault();
            }
            else
                throw new ApplicationException($"API Error: {result.StatusCode}");
        }
    }
}
