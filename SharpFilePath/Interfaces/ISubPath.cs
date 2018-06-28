namespace RoseByte.SharpFiles.Interfaces
{
    public interface ISubPath<out T>
    {
        string Value { get; }
        IFolder Parent { get; }
        T Child { get; }
    }
}