using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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

        public string GetLibraryAsync(string libName)
        {
            var result = _client.GetAsync("api/mediastore/libraries").Result;
            var json = result.Content.ReadAsStringAsync().Result;
            var libraries = JsonConvert.DeserializeObject<List<LibraryValue>>(responseBody);
            
        }

        public async Task<string> AddFileAsync(string filePath, string libraryId)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            ByteArrayContent bytes = new ByteArrayContent(fileBytes);
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            multiContent.Add(bytes, "file", Path.GetFileName(filePath));
            var result = await _client.PostAsync($"api/mediastore/libraries/{libraryId}/asset", multiContent);
            return result.Content.ReadAsStringAsync().Result;
        }
    }
}
