namespace SharpFilePath
{
    public class SubPath<T> : ISubPath<T>
    {
        public string Value { get; }
        public Folder Parent { get; }
        public T Child { get; }
        
        public SubPath(Folder parent, T child)
        {
            Parent = parent;
            Child = child;
            Value = child.ToString().Substring(parent.ToString().Length);
        }
        
        public override bool Equals(object obj) => Equals(obj as SubPath<T>);
        public override string ToString() => Value;
        public override int GetHashCode() => Value.GetHashCode();
        private bool Equals(SubPath<T> other) => string.Equals(Value, other.Value);
        
        public static bool operator ==(SubPath<T> left, SubPath<T> right) => left?.Equals(right) ?? right == null;
        public static bool operator !=(SubPath<T> left, SubPath<T> right) => !(left == right);
    }
}