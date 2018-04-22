using System;
using System.IO;

namespace SharpFilePath
{
	public abstract class SharpPath : IPath
	{
		protected readonly string Value;

		public SharpPath(string value)
		{
			Value = value;
		}

		public override string ToString()
		{
			return Value;
		}
		
		public static implicit operator SharpPath(string input)
		{
			return input.ToPath();
		}

		public static SharpPath FromString(string path)
		{
			var attributes = System.IO.File.GetAttributes(path);

			if (attributes.HasFlag(FileAttributes.Device) || path.EndsWith(":\\") || path.EndsWith(":"))
			{
				return new Drive(path);
			}
			
			if (attributes.HasFlag(FileAttributes.Directory))
			{
				return new Folder(path);
			}

			if (System.IO.File.Exists(path))
			{
				return new File(path);
			}
			
			return new EmptyPath(path);
		}
		
		public bool IsIn(SharpPath path) => Value.StartsWith(path.ToString());
	    
		public static bool operator <(SharpPath left, SharpPath right) => left.IsIn(right);
		public static bool operator >(SharpPath left, SharpPath right) => right.IsIn(left);
		public static bool operator ==(SharpPath left, SharpPath right) => left?.Equals(right) ?? right == null;
		public static bool operator !=(SharpPath left, SharpPath right) => !(left == right);
		public static bool operator <=(SharpPath left, SharpPath right) => left < right || left == right;
		public static bool operator >=(SharpPath left, SharpPath right) => left > right || left == right;

		public override bool Equals(object obj) => Equals(obj as SharpPath);
		public bool Equals(SharpPath other) => Value == other;

		public SharpPath Combine(string pathPart) => Path.Combine(Value, pathPart).ToPath();
		
		public override int GetHashCode() => (Value != null ? Value.GetHashCode() : 0);
		
		public SharpPath Resolve(SharpPath pwd)
		{
			if (pwd == null || pwd is EmptyPath)
			{
				throw new Exception($"Working directory demanded and not set: {Value}");
			}
			
			if (Value.StartsWith(".\\") || Value.StartsWith("./"))
			{
				return pwd.Combine(Value.Substring(1));
			}

			if (Value.StartsWith("..\\") || Value.StartsWith("../"))
			{
				return pwd.ParentDirectory.Combine(Value.Substring(2));
			}

			return this;
		}
		public IFolder ParentDirectory => new Folder(Directory.GetParent(Value).FullName);
		
		public abstract bool Exists { get; }
		public abstract void Remove();
		public abstract void Backup(SharpPath where, Action<long, long> progress);
		public abstract void Restore(SharpPath from, Action<long, long> progress);
		public abstract void Copy(SharpPath target, Action<long, long> progress);
	}
}