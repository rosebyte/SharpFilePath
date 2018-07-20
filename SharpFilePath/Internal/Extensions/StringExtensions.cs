using System;
using System.IO;
using RoseByte.SharpFiles.Internal;

namespace RoseByte.SharpFiles.Extensions
{
    public static class StringExtensions
    {
        public static FsObject ToPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
			
            if (Directory.Exists(path))
            {
                return new FolderImplementation(path);
            }
			
            if (System.IO.File.Exists(path))
            {
                return new FileImplementation(path);
            }
			
            throw new Exception($"Path can be either file or folder: {path}");
        }
        public static File ToFile(this string path) => new FileImplementation(path);
        public static Folder ToFolder(this string path) => new FolderImplementation(path);
    }
}