using System;
using System.Collections.Generic;

namespace RoseByte.SharpFiles.Interfaces
{
    public interface IFolder : IPath
    {
        void Mirror(IFolder target, Action<long, long, long> progress, IEnumerable<string> exceptions = null);
        IEnumerable<ISubPath<IFile>> GetFiles(bool recursive = true, string mask = null, 
            IEnumerable<string> exceptions = null);
        IEnumerable<ISubPath<IFolder>> GetFolders(bool recursive = true, string mask = null, 
            IEnumerable<string> exceptions = null);
        void CreateIfNotExists();
        void SyncStructure(IFolder destination, IEnumerable<string> exceptions = null);
        void Copy(IPath target, Predicate<ISubPath<IFile>> predicate);
    }
}