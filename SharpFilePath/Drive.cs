using System;

namespace SharpFilePath
{
    public class Drive : Folder
    {
        public Drive(string value) : base(value) { }

        public override void EnsureDirectoryCreated()
        {
            if (!Exists)
            {
                throw new Exception("Drive can't be created.");
            }
        }
    }
}