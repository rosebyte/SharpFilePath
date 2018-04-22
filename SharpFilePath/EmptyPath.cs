using System;

namespace SharpFilePath
{
    public class EmptyPath : Path
    {
        public EmptyPath(string value) : base(value) { }
        
        public override bool Exists => false;
        public override void Remove() { }
        public override void Copy(Path target, Action<long, long, long> progress) { }
    }
}