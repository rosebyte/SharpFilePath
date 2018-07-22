using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RoseByte.SharpFiles.Internal
{
    public class FilteredFolder : Folder
    {
        public bool Recursive { get; }
        public Regex Filter { get; }
        public Regex Skip { get; }

        internal FilteredFolder(string value, bool recursive, Regex filter, Regex skip) : base(value)
        {
            Recursive = recursive;
            Filter = filter;
            Skip = skip;
        }
        
        public override IEnumerable<FsChild<FsFile>> Files
        {
            get => new ScanHelper(this, Filter, Skip).GetFiles(Value, Recursive);
        }

        public override IEnumerable<FsChild<FsFolder>> Folders
        {
            get => new ScanHelper(this, Filter, Skip).GetFolders(Value, Recursive);
        }
    }
}