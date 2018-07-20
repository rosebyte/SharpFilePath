using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RoseByte.SharpFiles.Internal;

namespace RoseByte.SharpFiles
{
    public class ScanHelper
    {
        public Folder Base { get; }

        public ScanHelper(Folder folder)
        {
            Base = folder;
        }
        
        public IEnumerable<SubPath<File>> GetFiles(string path, bool recursive, string mask, IList<Regex> exceptions)
        {
            var files = Directory.EnumerateFiles(path, mask ?? "*", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var subpath = new SubPathImplementation<File>(Base, new FileImplementation(file));
                
                if (exceptions.Any(x => x.IsMatch(subpath.Value)))
                {
                    continue;
                }

                yield return subpath;
            }

            if (!recursive)
            {
                yield break;
            }

            var directories = Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);

            foreach (var directory in directories)
            {
                var subfolder = new SubPathImplementation<Folder>(Base, new FolderImplementation(directory));

                if (exceptions.Any(x => x.IsMatch(subfolder.Value)))
                {
                    continue;
                }
                
                foreach (var dirFile in GetFiles(directory, true, mask, exceptions))
                {
                    yield return dirFile;
                }
            }
        }
    }
}