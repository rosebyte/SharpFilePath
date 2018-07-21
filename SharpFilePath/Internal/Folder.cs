using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Internal
{
    public class Folder : FsFolder
    {
        internal Folder(string value) : base(value) { }
        
        public override bool Exists => Directory.Exists(Value);

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
            
            Directory.Delete(Value, true);
        }
        
        public override void Copy(FsPath target)
        {
            throw new NotImplementedException();
        }

        public override void Copy(FsPath target, Action<int> progress)
        {
            throw new NotImplementedException();
        }

        public override FsFile CombineFile(string pathPart) => new File(Path.Combine(this, pathPart));
        
        public override FsFolder CombineFolder(string pathPart) => new Folder(Path.Combine(Value, pathPart));
        
        public override void Copy(FsFolder target, Regex filter = null, Regex skip = null)
        {
            Copy(target, filter, skip, null);
        }
        
        public override void Copy(FsFolder target, Regex filter, Regex skip, Action<int> progress)
        {
            var helper = new ScanHelper(this, null, null);
            var folders = helper.GetFolders(this, true).ToList();
            var total = folders.Count;
            var last = 0;
            var counter = 0;
            
            progress?.Invoke(5);

            for (var i = 0; i < total; i++)
            {
                folders[i].Child.CreateIfNotExists();
                counter = 15 * i / total;

                if (counter > last)
                {
                    progress?.Invoke(counter);
                    last = counter;
                }
            }
            
            var files = helper.GetFiles(this, true).ToList();
            total = files.Count;

            for (var i = 0; i < total; i++)
            {
                folders[i].Copy(target);
                counter = 80 * i / total;

                if (counter > last)
                {
                    progress?.Invoke(counter);
                    last = counter;
                }
            }
        }

        public override FsFolder Parent => new Folder(Directory.GetParent(Value).FullName);

        public override void SyncStructure(FsFolder destination, Regex filter = null, Regex skip = null)
        {
            SyncStructure(destination, filter, skip, null);
        }
        
        public override void SyncStructure(FsFolder destination, Regex filter, Regex skip, Action<int> progress)
        {
            var helper = new ScanHelper(this, null, null);
            var destHelper = new ScanHelper(destination, null, null);
            var last = 0;
            var counter = 0;
            
            var folders = destHelper.GetFolders(this, true).ToList();
            var destFolders = destHelper.GetFolders(destination, true).Except(folders).ToList();
            var total = destFolders.Count;
            
            progress?.Invoke(5);
            
            for (var i = 0; i < total; i++)
            {
                destFolders[i].Child.Remove();
                counter = 5 * i / total;

                if (counter > last)
                {
                    progress?.Invoke(counter);
                    last = counter;
                }
            }
            
            var files = helper.GetFiles(this, true).ToList();
            var destFiles = destHelper.GetFiles(destination, true).Except(files).ToList();
            total = destFiles.Count;

            for (var i = 0; i < total; i++)
            {
                destFiles[i].Child.Remove();
                counter = 10 * i / total;

                if (counter > last)
                {
                    progress?.Invoke(counter);
                    last = counter;
                }
            }
            
            total = folders.Count;
            
            for (var i = 0; i < total; i++)
            {
                folders[i].Child.CreateIfNotExists();
                counter = 5 * i / total;

                if (counter > last)
                {
                    progress?.Invoke(counter);
                    last = counter;
                }
            }
            
            total = files.Count;

            for (var i = 0; i < total; i++)
            {
                files[i].Copy(destination);
                counter = 75 * i / total;

                if (counter > last)
                {
                    progress?.Invoke(counter);
                    last = counter;
                }
            }
        }

        public override IEnumerable<FsFile> GetFiles(bool recursive, Regex filter, Regex skip)
        {
            return new ScanHelper(this, filter, skip).GetFiles(Value, recursive).Select(x => x.Child);
        }

        public override IEnumerable<FsFolder> GetFolders(bool recursive, Regex filter, Regex skip)
        {
            return new ScanHelper(this, filter, skip).GetFolders(Value, recursive).Select(x => x.Child);
        }

        public override void CreateIfNotExists()
        {
            if (Exists)
            {
                return;
            }

            if (Parent == null)
            {
                throw new Exception("Top level node (e.g. drive) can't be created.");
            }
            
            if (!Parent.Exists)
            {
                Parent.CreateIfNotExists();
            }

            Directory.CreateDirectory(Value);
        }
    }
}