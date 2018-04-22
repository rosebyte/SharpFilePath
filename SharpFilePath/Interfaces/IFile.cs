using System;

namespace SharpFilePath.Interfaces
{
    public interface IFile : IPath
    {
        long Size { get; }
        DateTime Changed { get; }
        void Write(string content);
        string Content { get; }
        string NameWithoutExtension { get; }
        string Name { get; }
    }
}