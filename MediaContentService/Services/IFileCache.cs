using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaContentService.Services
{
	public interface IFileCache
	{
		long CurrentSize { get; }
		string CacheRootDirectory { get; }
		System.IO.DirectoryInfo GetDirectory(string dirName);
		string AllocateDirectory();
        void AllocateFile(string fileName, out string relPath, out string fullPath);
		long GetDirectorySpaceUsed(string dirPath);
    	void DeleteDirectory(string dirPath);
		void DeleteFile(string filePath);
		void Reset();
	}
}
