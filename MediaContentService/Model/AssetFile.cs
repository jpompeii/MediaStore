using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaContentService.Model
{
    public enum FileComponent
    {
        RootFile,
        DependentFile,
        Rendition,
        Thumbnail,
        Support
    };

    public class AssetFile
    {
        public string ResourceId { get; set; }
        public string SourceFileName { get; set; }
        public string MimeType { get; set; }
        public int Version { get; set; }
        public FileComponent ComponentType { get; set; }
    }
}
