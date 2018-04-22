using System.Collections.Generic;

namespace SharpFilePath
{
    public interface IFolder : IPath
    {
        void Mirror(IPath target);
        IEnumerable<SubPath<IFile>> GetFiles(bool recursive = true, string mask = null, 
            IEnumerable<string> exceptions = null);
        IEnumerable<SubPath<IFolder>> GetFolders(bool recursive = true, string mask = null, 
            IEnumerable<string> exceptions = null);
        void EnsureDirectoryCreated();
    }
}