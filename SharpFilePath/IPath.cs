using System;
using System.Collections.Generic;

namespace SharpFilePath
{
	public interface IPath
	{
		bool Exists { get; }
		void Remove();
		void Backup(SharpPath where, Action<long, long> progress);
		void Restore(SharpPath where, Action<long, long> progress);
		void Copy(SharpPath where, Action<long, long> progress);
		IFolder ParentDirectory { get; }
		SharpPath Combine(string childPath);
		SharpPath Resolve(SharpPath pwd);
	}
}