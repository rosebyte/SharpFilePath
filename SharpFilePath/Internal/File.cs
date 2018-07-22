using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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
            get => _hash ?? (_hash = SHA256.Create().ComputeHash(System.IO.File.ReadAllBytes(Value)));
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

        private void PrepareCopy(FsFile target)
        {
            target.Parent.Create();
            
            if (target.Exists && (System.IO.File.GetAttributes(Value) & FileAttributes.ReadOnly) != 0)
            {
                System.IO.File.SetAttributes(Value, FileAttributes.Normal);
            };
        }
        
        public override void Copy(FsFile target)
        {
            PrepareCopy(target);
            System.IO.File.Copy(Value, target, true);
        }
	    
        public override void Copy(FsFile target, Action<long, long> progress)
        {
            PrepareCopy(target);
            new FileEx(progress).Copy(this, target);
        }

        public override void Remove()
        {
            if (!Exists)
            {
                return;
            }
			
            try
            {
                if ((System.IO.File.GetAttributes(Value) & FileAttributes.ReadOnly) != 0)
                {
                    System.IO.File.SetAttributes(Value, FileAttributes.Normal);
                };
                
                System.IO.File.Delete(Value);
            }
            catch (Exception exception)
            {
                throw new Exception($"File '{Value}' could not be deleted: {exception.Message}");
            }
        }

        public override FsFolder Parent => Path.GetDirectoryName(Value).ToFolder();
        
        public override bool Exists => System.IO.File.Exists(Value);

        public override void Write(string content)
        {
            PrepareCopy(this);
            System.IO.File.WriteAllText(Value, content, Encoding.UTF8);
        }
    }
}