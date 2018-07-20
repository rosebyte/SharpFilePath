using System;

namespace RoseByte.SharpFiles
{
    public abstract class File : FsObject
    {
	    public abstract string Name { get; }
	    public abstract string NameWithoutExtension { get; }
	    public abstract string Content { get; }
	    public abstract byte[] Hash { get; }
	    public abstract DateTime Changed { get; }
	    public abstract long Size { get; }
	    protected File(string value) : base(value) { }
	    public abstract void Write(string content);
    }
}