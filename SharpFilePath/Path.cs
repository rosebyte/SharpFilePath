using System;
using System.IO;
using SharpFilePath.Interfaces;

namespace SharpFilePath
{
	public abstract class Path : IPath
	{
		protected readonly string Value;

		protected Path(string value)
		{
			Value = value;
		}

		public override string ToString()
		{
			return Value;
		}
		
		public static implicit operator string(Path input) => input.ToString();
		
		public static Path FromString(string path)
		{
			if (System.IO.File.Exists(path))
			{
				return new File(path);
			}

			if (Directory.Exists(path))
			{
				return new Folder(path);
			}
			
			return new EmptyPath(path);
		}

		private bool IsIn(Path path) => Value.StartsWith(path.ToString());
	    
		public static bool operator <(Path left, Path right) => left.IsIn(right);
		public static bool operator >(Path left, Path right) => right.IsIn(left);
		public static bool operator ==(Path left, Path right) => left?.Equals(right) ?? right == null;
		public static bool operator !=(Path left, Path right) => !(left == right);
		public static bool operator <=(Path left, Path right) => left < right || left == right;
		public static bool operator >=(Path left, Path right) => left > right || left == right;

		public override bool Equals(object obj) => Equals(obj as Path);
		private bool Equals(Path other)
		{
			if (ReferenceEquals(other, null))
			{
				return false;
			}

			return ReferenceEquals(Value, other.ToString());
		}

		public Path Combine(string pathPart) => System.IO.Path.Combine(Value, pathPart).ToPath();
		
		public override int GetHashCode() => (Value != null ? Value.GetHashCode() : 0);
		
		public Path Resolve(Path pwd)
		{
			if (pwd == null || pwd is EmptyPath)
			{
				throw new Exception($"Working directory demanded and not set: {Value}");
			}
			
			if (Value.StartsWith(".\\") || Value.StartsWith("./"))
			{
				return pwd.Combine(Value.Substring(2));
			}

			if (Value.StartsWith("..\\") || Value.StartsWith("../"))
			{
				return pwd.ParentDirectory.Combine(Value.Substring(3));
			}

			return this;
		}
		public IFolder ParentDirectory => new Folder(Directory.GetParent(Value).FullName);
		
		public abstract bool Exists { get; }
		public abstract void Remove();
		public abstract void Copy(Path target, Action<long, long, long> progress);
	}
}