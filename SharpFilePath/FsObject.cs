using System;
using System.IO;
using RoseByte.SharpFiles.Internal;

namespace RoseByte.SharpFiles
{
	public abstract class FsObject
	{
		protected readonly string Value;

		protected FsObject(string value)
		{
			Value = value;
		}
		
		public abstract Folder Parent { get; }
		public abstract FsObject Resolve(Folder pwd);
		public abstract FsObject Resolve();
		public abstract bool Exists { get; }
		public abstract void Remove();
		public abstract void Copy(FsObject target);
		public abstract void Copy(FsObject target, Action<int> progress);
		
		public static implicit operator string(FsObject input) => input.Value;
		public override string ToString() => Value;
		public override int GetHashCode() => (Value != null ? Value.GetHashCode() : 0);
		private bool IsIn(FsObject fsObject) => Value.StartsWith(fsObject.ToString());
	    public static bool operator <(FsObject left, FsObject right) => left.IsIn(right);
		public static bool operator >(FsObject left, FsObject right) => right.IsIn(left);
		public static bool operator ==(FsObject left, FsObject right) => left?.Equals(right) ?? right == null;
		public static bool operator !=(FsObject left, FsObject right) => !(left == right);
		public static bool operator <=(FsObject left, FsObject right) => left < right || left == right;
		public static bool operator >=(FsObject left, FsObject right) => left > right || left == right;
		public override bool Equals(object obj) => Equals(obj as FsObject);
		private bool Equals(FsObject other)
		{
			return !ReferenceEquals(other, null) && ReferenceEquals(Value, other.ToString());
		}
	}
}