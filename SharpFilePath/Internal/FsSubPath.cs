namespace RoseByte.SharpFiles
{
    public abstract class FsSubPath<T> where T : FsPath
    {
        public string Value { get; }
        public FsFolder Parent { get; }
        public T Child { get; }
        
        protected FsSubPath(FsFolder parent, T child)
        {
            Parent = parent;
            Child = child;
            Value = child.ToString().Substring(parent.ToString().Length + 1);
        }

        public abstract void Copy(FsFolder fsFolder);
        public bool IsFile => Child.IsFile;
        public bool IsFolder => Child.IsFolder;
        public void Remove() => Child.Remove();
        public override bool Equals(object obj) => Equals(obj as FsSubPath<T>);
        public override string ToString() => Value;
        public override int GetHashCode() => Value.GetHashCode();
        private bool Equals(FsSubPath<T> other) => string.Equals(Value, other.Value);
        public static bool operator ==(FsSubPath<T> left, FsSubPath<T> right) => left?.Equals(right) ?? right == null;
        public static bool operator !=(FsSubPath<T> left, FsSubPath<T> right) => !(left == right);
    }
}