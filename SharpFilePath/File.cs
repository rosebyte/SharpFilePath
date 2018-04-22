using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpFilePath.CopyFileEx;

namespace SharpFilePath
{
    public class File : SharpPath, IFile
    {
	    public string Name => Path.GetFileName(Value);
	    public string NameWithoutExtension => Path.GetFileNameWithoutExtension(Value);
	    
	    private string _content;
		public string Content => _content ?? (_content = System.IO.File.ReadAllText(Value));
	    
	    
	    private readonly Lazy<FileInfo> _fileInfo;
	    public DateTime Changed => _fileInfo.Value.LastWriteTime;
	    public long Size => _fileInfo.Value.Length;


	    public File(string value) : base(value)
	    {
		    _fileInfo = new Lazy<FileInfo>(() => new FileInfo(value));
	    }

	    public override void Copy(SharpPath target, Action<long, long> progress)
	    {
		    if (Size < 1000)
		    {
			    System.IO.File.Copy(Value, target.ToString(), true);
			    progress?.Invoke(Size, Size);
			    return;
		    }

		    var prog = 0;
		    
		    FileEx.Copy(Value, target.ToString(), ref prog, progress);
	    }

	    public string SubpathFrom(IPath path)
		{
			return Value.Substring(path.ToString().Length).TrimEnd('/', '\\');
		}

		public override void Remove()
		{
			System.IO.File.Delete(Value);
		}

	    public override void Backup(SharpPath where, Action<long, long> progress)
	    {
		    Copy(where, progress);
	    }

	    public override void Restore(SharpPath from, Action<long, long> progress)
	    {
		    from.Copy(this, progress);
	    }

	    public override bool Exists => System.IO.File.Exists(Value);

		public void Write(string content)
		{
			System.IO.File.WriteAllText(Value, content);
		}
    }
}