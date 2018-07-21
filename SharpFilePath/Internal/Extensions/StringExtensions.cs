using System;
using System.IO;
using RoseByte.SharpFiles.Internal;
using File = RoseByte.SharpFiles.Internal.File;

namespace RoseByte.SharpFiles.Extensions
{
    public static class StringExtensions
    {
        public static FsPath ToPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            
            path = Path.GetFullPath(path);
			
            if (Directory.Exists(path))
            {
                return new Folder(path);
            }
			
            if (System.IO.File.Exists(path))
            {
                return new File(path);
            }
			
            throw new Exception($"Unknown path: {path}");
        }

        public static FsFile ToFile(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            
            path = Path.GetFullPath(path);
            
            return new File(path);
        }

        public static FsFolder ToFolder(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            
            path = Path.GetFullPath(path);
            
            return new Folder(path);
        }
    }
}