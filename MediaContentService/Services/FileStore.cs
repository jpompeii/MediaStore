using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaContentService.Services
{
    public class FileStore : IFileStore
    {
        private IFileCache fileCache; 

        public FileStore(IFileCache fileCache)
        {
            this.fileCache = fileCache;
        }

        public Stream ReadFile(string fileUrl)
        {
            throw new NotImplementedException();
        }

        public string SaveFile(string fileId, Stream inStream)
        {
            string relPath = String.Empty;
            string fullPath = String.Empty;
            fileCache.AllocateFile(fileId, out relPath, out fullPath);
            FileStream outStream = new FileStream(fullPath, FileMode.CreateNew);
            inStream.CopyTo(outStream);
            return relPath;
        }
    }
}
