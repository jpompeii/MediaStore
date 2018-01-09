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
                    Library myLib = api.GetLibraryAsync(context).Result;
                    Console.WriteLine($"Library id for {context} is {myLib.LibraryKey}");

                    var asset = api.AddFileAsync(filePath, myLib.LibraryKey).Result;
                    Console.WriteLine($"Asset uploaded. (id={asset.Id})");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("usage: mscli get|put baseUrl libName|assetId filePath");
                Console.WriteLine("exception: " + ex);
                Environment.Exit(1);
            }
            
        }
    }
}
