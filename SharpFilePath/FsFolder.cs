using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RoseByte.SharpFiles
{
    public abstract class FsFolder : FsPath
    {
        protected FsFolder(string value) : base(value) { }
        public abstract FsFile CombineFile(string pathPart);
        public abstract FsFolder CombineFolder(string pathPart);
        public abstract void Copy(FsFolder target, Regex filter = null, Regex skip = null);
        public abstract void Copy(FsFolder target, Regex filter, Regex skip, Action<int> progress);
        public abstract void CreateIfNotExists();
        public override bool IsFile => false;
        public override bool IsFolder => true;
        public abstract void SyncStructure(FsFolder destination, Regex filter = null, Regex skip = null);
        public abstract void SyncStructure(FsFolder destination, Regex filter, Regex skip, Action<int> progress);
        public abstract IEnumerable<FsFile> GetFiles(bool recursive, Regex filter, Regex skip);
        public abstract IEnumerable<FsFolder> GetFolders(bool recursive, Regex filter, Regex skip);
    }
}