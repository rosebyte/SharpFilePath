namespace RoseByte.SharpFiles.Internal
{
    public class SubPath<T> : FsSubPath<T> where T : FsPath
    {
        public string Value { get; }
        public FsFolder Parent { get; }
        public T Child { get; }
        
        internal SubPath(FsFolder parent, T child) : base(parent, child)
        {
            Parent = parent;
            Child = child;
            Value = child.ToString().Substring(parent.ToString().Length + 1);
        }

        public override void Copy(FsFolder fsFolder) => Child.Copy(fsFolder.CombineFile(Value));
    }
}