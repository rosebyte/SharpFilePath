using System;

namespace SharpFilePath
{
    public class EmptyPath : Path
    {
        public EmptyPath(string value) : base(value) { }
        
        public override bool Exists => false;
        public override void Remove() { }
        public override void Backup(Path @where, Action<long, long, long> progress) { }
        public override void Restore(Path @from, Action<long, long, long> progress) { }
        public override void Copy(Path target, Action<long, long, long> progress) { }
    }
}