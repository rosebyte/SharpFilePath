using System;
using System.Collections.Generic;

namespace SharpFilePath
{
	public interface IPath
	{
		bool Exists { get; }
		void Remove();
		void Backup(Path where, Action<long, long, long> progress);
		void Restore(Path where, Action<long, long, long> progress);
		void Copy(Path where, Action<long, long, long> progress);
		IFolder ParentDirectory { get; }
		Path Combine(string childPath);
		Path Resolve(Path pwd);
	}
}