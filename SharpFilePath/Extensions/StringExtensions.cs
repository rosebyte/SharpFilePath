namespace RoseByte.SharpFiles.Extensions
{
    public static class StringExtensions
    {
        public static Path ToPath(this string path) => Path.FromString(path);
        public static Path ToFile(this string path) => Path.FromString(path) as File;
        public static Path ToFolder(this string path) => Path.FromString(path) as Folder;
    }
}