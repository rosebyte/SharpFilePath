using System;
using System.IO;
using System.Security.Cryptography;
using RoseByte.SharpFiles.CopyFileEx;
using RoseByte.SharpFiles.Interfaces;

namespace RoseByte.SharpFiles
{
    public class File : Path, IFile
    {
	    public string Name => System.IO.Path.GetFileName(Value);
	    public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Value);
	    
	    private string _content;
		public string Content => _content ?? (_content = System.IO.File.ReadAllText(Value));

	    public byte[] Hash => SHA256.Create().ComputeHash(System.IO.File.ReadAllBytes(Value));
	    
	    private readonly Lazy<FileInfo> _fileInfo;
	    public DateTime Changed => _fileInfo.Value.LastWriteTime;
	    public long Size => _fileInfo.Value.Length;


	    public File(string value) : base(value)
	    {
		    _fileInfo = new Lazy<FileInfo>(() => new FileInfo(value));
	    }

	    public override void Copy(IPath target, Action<long, long, long> progress)
	    {
		    if (!(target is IFile))
		    {
			    throw new Exception("File can be copied only to file path, not");
		    }
		    
		    if (Size < 1000)
		    {
			    System.IO.File.Copy(Value, target.ToString(), true);
			    progress?.Invoke(Size, Size, Size);
			    return;
		    }

		    var prog = 0;
		    
		    new FileEx(progress).Copy(Value, target.ToString(), ref prog);
	    }

	    public override void Remove()
		{

			if ((System.IO.File.GetAttributes(Value) & FileAttributes.ReadOnly) != 0)
			{
				System.IO.File.SetAttributes(Value, FileAttributes.Normal);
			};
			System.IO.File.Delete(Value);
		}

	    public override bool Exists => System.IO.File.Exists(Value);

		public void Write(string content)
		{
			System.IO.File.WriteAllText(Value, content);
		}
    }
}