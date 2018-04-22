using SharpFilePath;

namespace System
{
    public static class StringExtensions
    {
        public static Path ToPath(this string path) => Path.FromString(path);
    }
}