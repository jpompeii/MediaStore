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

        public string CreateFileId(string fileName = null)
        {
            if (fileName == null)
                fileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());

            string relPath = String.Empty;
            string fullPath = String.Empty;
            fileCache.AllocateFile(fileName, out relPath, out fullPath);
            return relPath;
        }

        public string SaveFile(string fileId, Stream inStream)
        {
            string fullPath = fileCache.GetDirectory(fileId).ToString();
            FileStream outStream = new FileStream(fullPath, FileMode.CreateNew);
            inStream.CopyTo(outStream);
            return fileId;
        }
    }
}
