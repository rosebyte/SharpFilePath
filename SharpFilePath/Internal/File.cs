using System;
using System.IO;
using System.Security.Cryptography;
using RoseByte.SharpFiles.CopyFileEx;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Internal
{
    public class File : FsFile
    {
        public override string Name => Path.GetFileName(Value);
        public override string NameWithoutExtension => Path.GetFileNameWithoutExtension(Value);
	    
        private string _content;
        public override string Content => _content ?? (_content = System.IO.File.ReadAllText(Value));

        private byte[] _hash;
        public override byte[] Hash
        {
            get { return _hash ?? (_hash = SHA256.Create().ComputeHash(System.IO.File.ReadAllBytes(Value))); }
        }

        private long? _size;
        public override long Size
        {
            get
            {
                if (!_size.HasValue)
                {
                    _size = new FileInfo(Value).Length;
                }

                return _size.Value;
            }
        }


        internal File(string value) : base(value) { }

        public override void Copy(FsPath target)
        {
            if (!(target is File))
            {
                throw new Exception("File can be copied only to file path, not");
            }

            target.Parent.CreateIfNotExists();
            System.IO.File.Copy(Value, target, true);
        }
	    
        public override void Copy(FsPath target, Action<int> progress)
        {
            if (!(target is File))
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

        public override FsFolder Parent => Path.GetDirectoryName(Value).ToFolder();
        
        public override bool Exists => System.IO.File.Exists(Value);

        public override void Write(string content)
        {
            System.IO.File.WriteAllText(Value, content);
        }
    }
}