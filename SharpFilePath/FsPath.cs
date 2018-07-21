using System;
using System.IO;
using RoseByte.SharpFiles.Internal;

namespace RoseByte.SharpFiles
{
	public abstract class FsPath
	{
		public readonly string Value;

		protected FsPath(string value)
		{
			Value = value;
		}
		
		public abstract bool IsFile { get; }
		public abstract bool IsFolder { get; }
		public abstract FsFolder Parent { get; }
		public abstract bool Exists { get; }
		public abstract void Remove();
		public abstract void Copy(FsPath target);
		public abstract void Copy(FsPath target, Action<int> progress);
		
		public static implicit operator string(FsPath input) => input.Value;
		public override string ToString() => Value;
		public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
		private bool IsIn(FsPath fsPath) => Value.StartsWith(fsPath.ToString());
	    public static bool operator <(FsPath left, FsPath right) => left.IsIn(right);
		public static bool operator >(FsPath left, FsPath right) => right.IsIn(left);
		public static bool operator ==(FsPath left, FsPath right) => left?.Equals(right) ?? right == null;
		public static bool operator !=(FsPath left, FsPath right) => !(left == right);
		public static bool operator <=(FsPath left, FsPath right) => left < right || left == right;
		public static bool operator >=(FsPath left, FsPath right) => left > right || left == right;

		public override bool Equals(object obj) => (obj is string str && str == Value) || Equals(obj as FsPath);
		private bool Equals(FsPath other) => !ReferenceEquals(other, null) && string.Equals(Value, other.Value);
	}
}