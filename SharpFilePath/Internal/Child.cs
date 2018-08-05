namespace RoseByte.SharpFiles.Internal
{
    public class Child<T> : FsChild<T> where T : FsPath
    {
        internal Child(FsFolder parent, T child) : base(parent, child) { }
    }
}