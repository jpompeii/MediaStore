using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaContentService.Services
{
	public class FileCache : IFileCache
	{
		private string _rootDir;
		private string _hostRoot;
		private int[] _currentPath;
		private int[] _dirCounts;
		private int _itemCountThreshold;
        private int _currentFileCount;
		private static string _sep = @"\";
		private static readonly object _lockObject = new object();
        public static int DIRDEPTH = 3; 

		public string CacheRootDirectory
		{
			get
			{
				return _rootDir; 
			}
		}

		public long CurrentSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public FileCache(IConfiguration config)
		{
			_rootDir = config[""].TrimEnd();
			_itemCountThreshold = 1024;

			if (!_rootDir.EndsWith(_sep))
				_rootDir += _sep;

			_hostRoot = _rootDir;
			_currentPath = new int[DIRDEPTH];
			_dirCounts = new int[DIRDEPTH];
            _currentFileCount = -1;
			DetermineStartDirectory();
		}

		private void DetermineStartDirectory()
		{
			string path = _hostRoot;
			for (int i = 0; i < _currentPath.Length; ++i)
			{
				int count;
				int currentDir = GetCurrentSubDir(path, out count);
				_currentPath[i] = currentDir;
				_dirCounts[i] = count;
				if (count == 0 && i < _currentPath.Length - 1)
				{
					Directory.CreateDirectory(path + "0");
				}
				path += currentDir.ToString() + _sep;
			}
		}

		private int GetCurrentSubDir(string dir, out int count)
		{
			int currentDir = 0;
			int itemCount = 0;
			if (Directory.Exists(dir))
			{ 
				IEnumerable<string> subdirs = Directory.EnumerateDirectories(dir);
				foreach (var subdir in subdirs)
				{
					int intValue;
					string dirName = subdir.Substring(subdir.LastIndexOf(_sep) + 1);
					if (Int32.TryParse(dirName, out intValue))
					{
						itemCount++;
						if (intValue > currentDir)
							currentDir = intValue;
					}
				}
			}
			count = itemCount;
			return currentDir;
		}

		public string AllocateDirectory()
		{
			int retryCount = 3;

			do
			{
				string relPath;
				if (TryNextDirectory(out relPath))
				{
					Directory.CreateDirectory(_rootDir + relPath);
					return relPath;
				}
			} while (--retryCount > 0);

			throw new ApplicationException("Cannot allocate unique directory");
		}

        public void AllocateFile(string fileName, out string relPath, out string fullPath)
        {
            var currRelPath = CurrentRelativePath();
            var currPath = $"{_rootDir}{currRelPath}";
            var dir = new DirectoryInfo(currPath);

            if (_currentFileCount < 0 || (_currentFileCount % 100) == 0)
            {
                _currentFileCount = Directory.EnumerateFiles(currPath).AsParallel().Count();
            }
            if (_currentFileCount > _itemCountThreshold)
            {
                TryNextDirectory(out currRelPath);
                _currentFileCount = Directory.EnumerateFiles($"{_rootDir}{currRelPath}").AsParallel().Count();
            }
            relPath = $"{currRelPath}{_sep}{fileName}";
            fullPath = $"{_rootDir}{relPath}";
        }

        public string CurrentRelativePath()
        {
            string path = "";
            for (int i = 0; i < _currentPath.Length; ++i)
            {
                path += _currentPath[i].ToString() + _sep;
            }
            return path;
        }

        private bool TryNextDirectory(out string relPath)
		{
			lock (_lockObject)
			{
				for (int i = _currentPath.Length - 1; i >= 0; --i)
				{
					++_currentPath[i];
					++_dirCounts[i];
					if (i > 0 && _dirCounts[i] >= _itemCountThreshold)
					{
						_currentPath[i] = 0;
						_dirCounts[i] = 0;
					}
					else break;
				}

				string path = "";
				for (int i = 0; i < _currentPath.Length; ++i)
				{
					path += _currentPath[i].ToString() + _sep;
				}
				relPath = path;
				return !Directory.Exists(_rootDir + path);
			}
		}

		public void Reset()
		{
			Directory.Delete(_hostRoot, true);
			DetermineStartDirectory();
		}

		public void DeleteDirectory(string relPath)
		{
			GetDirectory(relPath).Delete(true);
		}

		public void DeleteFile(string filePath) => File.Delete(filePath);

		public DirectoryInfo GetDirectory(string relPath)
		{
			return new DirectoryInfo(_rootDir + relPath);
		}

		public long GetDirectorySpaceUsed(string relPath)
		{
			return GetDirectory(relPath).EnumerateFiles("*", SearchOption.AllDirectories).Sum(x => x.Length);
		}

    }
}
