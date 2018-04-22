namespace SharpFilePath.Interfaces
{
    public interface ISubPath<out T>
    {
        string Value { get; }
        Folder Parent { get; }
        T Child { get; }
    }
}