using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RoseByte.SharpFiles
{
    public abstract class Folder : FsObject
    {
        protected Folder(string value) : base(value) { }
        public abstract void Copy(Folder target);
        public abstract FsObject Combine(string pathPart);
        public abstract Folder GetSubFolder(string pathPart);
        public abstract void Copy(Folder target, Predicate<SubPath<File>> predicate);
        public abstract void Copy(List<Regex> skip, Folder target, Action<int> progress = null);
        public abstract void CreateIfNotExists();
        
        public abstract void SyncStructure(
            Folder destination, IList<Regex> exceptions = null, Action<int> progress = null);

        public abstract IEnumerable<SubPath<File>> GetFiles(bool recursive = true, string mask = null,
            IList<Regex> exceptions = null);

        public abstract IEnumerable<SubPath<Folder>> GetFolders(
            bool recursive = true, string mask = null, IList<Regex> exceptions = null);
    }
}