using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaContentService.Services
{
    public class Mime
    {
        // public static Dictionary<string, string> mimeMappings = null;

        public static string GetMimeFromExt(string ext)
        {
            ext = ext.ToLower();
            if (ext.StartsWith('.'))
                ext = ext.Substring(1);

            if (ext == "jpg")
                return "image/jpeg";
            else if (ext == "png")
                return "image/png";
            else if (ext == "mp3")
                return "audio/mp3";
            else
                return "application/octet-stream";
        }
    }
}
