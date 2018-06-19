namespace RoseByte.SharpFiles.Extensions
{
    public static class StringExtensions
    {
        public static Path ToPath(this string path) => Path.FromString(path);
        public static File ToFile(this string path) => (File)Path.FromString(path);
        public static Folder ToFolder(this string path) => (Folder)Path.FromString(path);
    }
}