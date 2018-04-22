using System.Collections.Generic;

namespace SharpFilePath
{
    public class SubPath<T>
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubPath<T>) obj);
        }

        public string Value { get; }
        public Folder Parent { get; }
        public T Child { get; }

        public SubPath(Folder parent, T child)
        {
            Parent = parent;
            Child = child;
            Value = child.ToString().Substring(parent.ToString().Length);
        }

        public override string ToString() => Value;
        public override int GetHashCode() => Value.GetHashCode();
        
        public static bool operator ==(SubPath<T> left, SubPath<T> right) => left?.Equals(right) ?? right == null;
        public static bool operator !=(SubPath<T> left, SubPath<T> right) => !(left == right);
        protected bool Equals(SubPath<T> other) => string.Equals(Value, other.Value);
    }
}