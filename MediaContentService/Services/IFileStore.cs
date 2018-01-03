using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaContentService.Services
{
    public interface IFileStore
    {
        string SaveFile(string fileId, Stream inStream);
        Stream ReadFile(string fileUrl);
    }
}
