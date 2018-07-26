using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Internal
{
    public class FilteredFolder : Folder
    {
        public bool Recursive { get; }
        public Regex FilterFiles { get; }
        public Regex SkipFiles { get; }
        public Regex FilterFolders { get; }
        public Regex SkipFolders { get; }

        internal FilteredFolder(string value, bool recursive, Regex filterFolders, Regex skipFolders, Regex filterFiles,
            Regex skipFiles) : base(value)
        {
            Recursive = recursive;
            FilterFiles = filterFiles;
            FilterFolders = filterFolders;
            SkipFiles = skipFiles;
            SkipFolders = skipFolders;
        }
        
        public override IEnumerable<FsChild<FsFile>> Files => GetFiles(Value, Recursive);

        public override IEnumerable<FsChild<FsFolder>> Folders => GetFolders(Value, Recursive);

        private IEnumerable<FsChild<FsFolder>> GetFolders(string path, bool recursive)
        {
            var files = Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
            
            foreach (var file in files)
            {
                var subpath = new FsChild<FsFolder>(this, new Folder(file));
                
                if (!FilterFolders?.IsMatch(subpath.Value) ?? false)
                {
                    continue;
                }
                
                if (SkipFolders?.IsMatch(subpath.Value) ?? false)
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
        
        private IEnumerable<FsChild<FsFile>> GetFiles(string path, bool recursive)
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
                    var subpath = new FsChild<FsFile>(this, new File(file));
                    
                    if (!FilterFiles?.IsMatch(subpath.Value) ?? false)
                    {
                        continue;
                    }
                
                    if (SkipFiles?.IsMatch(subpath.Value) ?? false)
                    {
                        continue;
                    }
                
                    yield return subpath;
                }
            }
        }
    }
}