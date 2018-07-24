namespace RoseByte.SharpFiles
{
    public class FsChild<T> where T : FsPath
    {
        public string Value { get; }
        public FsFolder Parent { get; }
        public T Child { get; }
        
        internal FsChild(FsFolder parent, T child)
        {
            Parent = parent;
            Child = child;
            Value = child.Value.Substring(parent.Value.Length + 1);
        }
        
        public bool IsFile => Child.IsFile;
        public bool IsFolder => Child.IsFolder;
        public override bool Equals(object obj) => Equals(obj as FsChild<T>);
        public override string ToString() => Value;
        public override int GetHashCode() => Value.GetHashCode();
        private bool Equals(FsChild<T> other) => string.Equals(Value, other.Value);
        public static bool operator ==(FsChild<T> left, FsChild<T> right) => left?.Equals(right) ?? right == null;
        public static bool operator !=(FsChild<T> left, FsChild<T> right) => !(left == right);
    }
}