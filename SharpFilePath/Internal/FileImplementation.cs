using System;
using System.IO;
using System.Security.Cryptography;
using RoseByte.SharpFiles.CopyFileEx;

namespace RoseByte.SharpFiles.Internal
{
    public class FileImplementation : File
    {
        public override string Name => Path.GetFileName(Value);
        public override string NameWithoutExtension => Path.GetFileNameWithoutExtension(Value);
	    
        private string _content;
        public override string Content => _content ?? (_content = System.IO.File.ReadAllText(Value));

        public override byte[] Hash => SHA256.Create().ComputeHash(System.IO.File.ReadAllBytes(Value));
	    
        private readonly Lazy<FileInfo> _fileInfo;
        public override DateTime Changed => _fileInfo.Value.LastWriteTime;
        public override long Size => _fileInfo.Value.Length;


        internal FileImplementation(string value) : base(value)
        {
            _fileInfo = new Lazy<FileInfo>(() => new FileInfo(value));
        }

        public override void Copy(FsObject target)
        {
            if (!(target is FileImplementation))
            {
                throw new Exception("File can be copied only to file path, not");
            }

            target.Parent.CreateIfNotExists();
            System.IO.File.Copy(Value, target, true);
        }
	    
        public override void Copy(FsObject target, Action<int> progress)
        {
            if (!(target is FileImplementation))
            {
                throw new Exception("File can be copied only to file path, not");
            }
		    
            var prog = 0;
		    
            new FileEx(progress).Copy(Value, target.ToString(), ref prog);
        }

        public override void Remove()
        {
            if (!Exists)
            {
                return;
            }
			
            if ((System.IO.File.GetAttributes(Value) & FileAttributes.ReadOnly) != 0)
            {
                System.IO.File.SetAttributes(Value, FileAttributes.Normal);
            };
            System.IO.File.Delete(Value);
        }

        public override Folder Parent { get; }
        public FsObject Resolve(FolderImplementation pwd)
        {
            throw new NotImplementedException();
        }

        public override FsObject Resolve(Folder pwd)
        {
            if (pwd == null)
            {
                throw new Exception($"Working directory demanded and not set: {Value}");
            }
			
            if (Value.StartsWith(".\\") || Value.StartsWith("./"))
            {
                return new FileImplementation(Path.Combine(pwd, Value.Substring(2)));
            }

            if (Value.StartsWith("..\\") || Value.StartsWith("../"))
            {
                return pwd.Parent.Combine(Value.Substring(3));
            }

            return this;
        }

        public override FsObject Resolve()
        {
            throw new NotImplementedException();
        }

        public override bool Exists => System.IO.File.Exists(Value);

        public override void Write(string content)
        {
            System.IO.File.WriteAllText(Value, content);
        }
    }
}