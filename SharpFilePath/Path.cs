using System;
using System.IO;

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
		
		public static implicit operator Path(string input)
		{
			return input.ToPath();
		}

		public static Path FromString(string path)
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

		private bool IsIn(Path path) => Value.StartsWith(path.ToString());
	    
		public static bool operator <(Path left, Path right) => left.IsIn(right);
		public static bool operator >(Path left, Path right) => right.IsIn(left);
		public static bool operator ==(Path left, Path right) => left?.Equals(right) ?? right == null;
		public static bool operator !=(Path left, Path right) => !(left == right);
		public static bool operator <=(Path left, Path right) => left < right || left == right;
		public static bool operator >=(Path left, Path right) => left > right || left == right;

		public override bool Equals(object obj) => Equals(obj as Path);
		private bool Equals(Path other) => Value == other;

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
		public abstract void Backup(Path where, Action<long, long, long> progress);
		public abstract void Restore(Path from, Action<long, long, long> progress);
		public abstract void Copy(Path target, Action<long, long, long> progress);
	}
}