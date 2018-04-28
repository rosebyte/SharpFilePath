using System;

namespace RoseByte.SharpFiles.Interfaces
{
	public interface IPath
	{
		bool Exists { get; }
		void Remove();
		void Copy(Path where, Action<long, long, long> progress);
		IFolder ParentDirectory { get; }
		Path Combine(string childPath);
		Path Resolve(Path pwd);
	}
}