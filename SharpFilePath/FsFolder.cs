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
        public abstract void Create();
        public override bool IsFile => false;
        public override bool IsFolder => true;
        public abstract void SyncStructure(FsFolder destination, bool force);
        public abstract IEnumerable<FsChild<FsFile>> Files { get; }
        public abstract IEnumerable<FsChild<FsFolder>> Folders { get; }
        public abstract bool Recursive { get; }
        public abstract Regex FilesFilter { get; }
        public abstract Regex FilesSkip { get; }
        public abstract Regex FoldersFilter { get; }
        public abstract Regex FoldersSkip { get; }
        public abstract FsFolder SetFileSkip(Regex rgx);
        public abstract FsFolder SetFileFilter(Regex rgx);
        public abstract FsFolder SetFolderSkip(Regex rgx);
        public abstract FsFolder SetFolderFilter(Regex rgx);
        public abstract FsFolder SetRecursivity(bool recursivity);
        public abstract void Copy(FsChild<FsFile> child);
        public abstract void Create(FsChild<FsFolder> child);
        public abstract void Remove(FsChild<FsFolder> child);
        public abstract void Remove(FsChild<FsFile> child);
    }
}