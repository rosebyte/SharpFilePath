using System;

namespace RoseByte.SharpFiles
{
    public abstract class FsFile : FsPath
    {
	    public abstract string Name { get; }
	    public abstract string NameWithoutExtension { get; }
	    public abstract string Content { get; }
	    protected FsFile(string value) : base(value) { }
	    public abstract void Write(string content);
	    public override bool IsFile => true;
	    public override bool IsFolder => false;
	    public abstract long Size { get; }
	    public abstract byte[] Hash { get; }
	    public abstract void Copy(FsFile target);
	    public abstract void Copy(FsFile target, Action<long, long> progress);
    }
}