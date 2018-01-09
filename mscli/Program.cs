using System;

namespace MediaStore.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                char mode;

                if (args[0] == "get")
                    mode = 'g';
                else if (args[0] == "put")
                    mode = 'p';
                else
                    throw new ArgumentException();

                var baserUrl = args[1];
                var context = args[2];
                var filePath = args[3];

                var api = new MediaStoreClient("http://localhost:32076");
                if (mode == 'p')
                {
                    Library myLib = api.GetLibraryAsync("media").Result;
                    Console.WriteLine($"Library id for media is {myLib.LibraryKey}");

                    var asset = api.AddFileAsync(filePath, myLib.LibraryKey).Result;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("usage: mscli get|put baseUrl libName|assetId filePath");
                Console.WriteLine("exception: " + ex);
                Environment.Exit(1);
            }

            // usage: mscli get|put baseUrl libName|assetId filePath 
            
        }
    }
}
