using System;

namespace RoseByte.SharpFiles.Interfaces
{
	public interface IPath
	{
		bool Exists { get; }
		void Remove();
		void Copy(IPath where, Action<long, long, long> progress);
		void Copy(IPath where);
		IFolder ParentDirectory { get; }
		IPath Combine(string childPath);
		IPath Resolve(Path pwd);
		IFolder GetSubFolder(string pathPart);
	}
}