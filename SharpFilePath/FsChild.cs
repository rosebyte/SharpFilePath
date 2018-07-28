namespace RoseByte.SharpFiles
{
    public class FsChild<T> where T : FsPath
    {
        /// <summary>
        /// All the path after parent's part
        /// </summary>
        public string SubPath { get; }
        public FsFolder Parent { get; }
        public T Child { get; }
        
        internal FsChild(FsFolder parent, T child)
        {
            Parent = parent;
            Child = child;
            SubPath = child.Path.Substring(parent.Path.Length ).Trim('\\', '/');
        }
        
        public bool IsFile => Child.IsFile;
        public bool IsFolder => Child.IsFolder;
        public override bool Equals(object obj) => Equals(obj as FsChild<T>);
        public override string ToString() => SubPath;
        public override int GetHashCode() => SubPath.GetHashCode();
        private bool Equals(FsChild<T> other) => string.Equals(SubPath, other.SubPath);
        public static bool operator ==(FsChild<T> left, FsChild<T> right) => left?.Equals(right) ?? right == null;
        public static bool operator !=(FsChild<T> left, FsChild<T> right) => !(left == right);
    }
}