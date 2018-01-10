using System;
using System.IO;

namespace MediaContentService.Services
{
    public class FileStore : IFileStore
    {
        private IFileCache fileCache; 

        public FileStore(IFileCache fileCache)
        {
            this.fileCache = fileCache;
        }

        public Stream ReadFile(string fileId)
        {
            string fullPath = fileCache.GetDirectory(fileId).ToString();
            return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        }

        public string CreateFileId(string fileName = null)
        {
            if (fileName == null)
                fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

            string relPath = String.Empty;
            string fullPath = String.Empty;
            fileCache.AllocateFile(fileName, out relPath, out fullPath);
            return relPath;
        }

        public string SaveFile(string fileId, Stream inStream)
        {
            string fullPath = fileCache.GetDirectory(fileId).ToString();
            using (FileStream outStream = new FileStream(fullPath, FileMode.CreateNew))
                inStream.CopyTo(outStream);

            return fileId;
        }
    }
}
