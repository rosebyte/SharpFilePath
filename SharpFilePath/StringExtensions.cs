namespace SharpFilePath
{
    public static class StringExtensions
    {
        public static SharpPath ToPath(this string path) => SharpPath.FromString(path);
    }
}