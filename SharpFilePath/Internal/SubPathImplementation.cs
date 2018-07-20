namespace RoseByte.SharpFiles.Internal
{
    public class SubPathImplementation<T> : SubPath<T> where T : FsObject
    {
        public string Value { get; }
        public Folder Parent { get; }
        public T Child { get; }
        
        internal SubPathImplementation(Folder parent, T child) : base(parent, child)
        {
            Parent = parent;
            Child = child;
            Value = child.ToString().Substring(parent.ToString().Length + 1);
        }

        public override void Copy(Folder folder) => Child.Copy(folder.Combine(Value));
    }
}