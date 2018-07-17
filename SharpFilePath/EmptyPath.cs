using System;
using RoseByte.SharpFiles.Interfaces;

namespace RoseByte.SharpFiles
{
    public class EmptyPath : Path
    {
        public EmptyPath(string value) : base(value) { }

        public override void Copy(IPath target) { }
        public override bool Exists => false;
        public override void Remove() { }
        public override void Copy(IPath target, Action<long, long, long> progress) { }
    }
}