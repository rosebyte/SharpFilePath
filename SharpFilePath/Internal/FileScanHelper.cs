using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RoseByte.SharpFiles.Extensions;
using RoseByte.SharpFiles.Internal;
using File = RoseByte.SharpFiles.Internal.File;

namespace RoseByte.SharpFiles
{
    public class ScanHelper
    {
        private FsFolder Base { get; }
        private readonly Regex _filter;
        private readonly Regex _skip;

        public ScanHelper(FsFolder fsFolder, Regex filter, Regex skip)
        {
            Base = fsFolder;
            _filter = filter;
            _skip = skip;
        }

        public IEnumerable<FsSubPath<FsFolder>> GetFolders(string path, bool recursive)
        {
            var files = Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
            
            foreach (var file in files)
            {
                var subpath = new SubPath<FsFolder>(Base, new Folder(file));
                
                if (!_filter?.IsMatch(subpath.Value) ?? false)
                {
                    continue;
                }
                
                if (_skip?.IsMatch(subpath.Value) ?? false)
                {
                    continue;
                }
                
                yield return subpath;

                if (recursive)
                {
                    foreach (var subFolder in GetFolders(subpath.Child, true))
                    {
                        yield return subFolder;
                    }
                }
            }
        }
        
        public IEnumerable<FsSubPath<FsFile>> GetFiles(string path, bool recursive)
        {
            IEnumerable<FsFolder> folders = new []{path.ToFolder()};

            if (recursive)
            {
                folders = folders.Union(GetFolders(path, true).Select(x => x.Child));
            }

            foreach (var folder in folders)
            {
                var files = Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly);

                foreach (var file in files)
                {
                    var subpath = new SubPath<FsFile>(Base, new File(file));
                    
                    if (!_filter?.IsMatch(subpath.Value) ?? false)
                    {
                        continue;
                    }
                
                    if (_skip?.IsMatch(subpath.Value) ?? false)
                    {
                        continue;
                    }
                
                    yield return subpath;
                }
            }
        }
    }
}