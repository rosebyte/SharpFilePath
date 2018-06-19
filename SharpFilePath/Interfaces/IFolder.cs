using System;
using System.Collections.Generic;

namespace RoseByte.SharpFiles.Interfaces
{
    public interface IFolder : IPath
    {
        void Mirror(IFolder target, Action<long, long, long> progress);
        IEnumerable<ISubPath<IFile>> GetFiles(bool recursive = true, string mask = null, 
            IEnumerable<string> exceptions = null);
        IEnumerable<ISubPath<IFolder>> GetFolders(bool recursive = true, string mask = null, 
            IEnumerable<string> exceptions = null);
        void CreateIfNotExists();
    }
}