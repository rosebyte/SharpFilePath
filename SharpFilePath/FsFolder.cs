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
        public abstract void SyncStructure(FsFolder destination);
        public abstract IEnumerable<FsChild<FsFile>> Files { get; }
        public abstract IEnumerable<FsChild<FsFolder>> Folders { get; }
        public abstract FsFolder Filter(bool recursive, Regex filterFolders, Regex skipFolders, Regex filterFiles, 
            Regex skipFiles);
    }
}