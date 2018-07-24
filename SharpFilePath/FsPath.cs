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
		
		private long? _size;
		public long Size
		{
			get
			{
				if (_size == null)
				{
					_size = GetSize();
				}

				return _size.Value;
			}

			protected set => _size = value;
		}
		
		public abstract bool IsFile { get; }
		public abstract bool IsFolder { get; }
		public abstract FsFolder Parent { get; }
		public abstract bool Exists { get; }
		public abstract void Remove();
		protected abstract long GetSize();
		public void RefreshSize() => _size = GetSize();
		
		public static implicit operator string(FsPath input) => input.Value;
		public override string ToString() => Value;
		public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
		
		public override bool Equals(object obj) => (obj is string str && str == Value) || Equals(obj as FsPath);
		public bool Equals(FsPath other) => !ReferenceEquals(other, null) && string.Equals(Value, other.Value);
	    public static bool operator ==(FsPath left, FsPath right) => left?.Equals(right) ?? right == null;
		public static bool operator !=(FsPath left, FsPath right) => !(left == right);
	}
}