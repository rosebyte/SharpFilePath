using System;

namespace SharpFilePath
{
    public class EmptyPath : SharpPath
    {
        public EmptyPath(string value) : base(value) { }
        
        public override bool Exists => false;
        public override void Remove() { }
        public override void Backup(SharpPath @where, Action<int> progress) { }
        public override void Restore(SharpPath @from, Action<int> progress) { }
        public override void Copy(SharpPath target, Action<int> progress) { }
    }
}