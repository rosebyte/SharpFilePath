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
        public override FsFile CombineFile(string pathPart) => new File(Path.Combine(this, pathPart));       
        public override FsFolder CombineFolder(string pathPart) => new Folder(Path.Combine(Value, pathPart));
        public override FsFolder Parent => new Folder(Directory.GetParent(Value).FullName);
        public override IEnumerable<FsChild<FsFile>> Files => new ScanHelper(this).GetFiles(Value, true);
        public override IEnumerable<FsChild<FsFolder>> Folders => new ScanHelper(this).GetFolders(Value, true);

        public override FsFolder Filter(bool recursive, Regex filter, Regex skip)
        {
            return new FilteredFolder(this, recursive, filter, skip);
        }

        public void Copy(FsChild<FsFile> child) => child.Child.Copy(CombineFile(child.Value));
        public void Create(FsChild<FsFolder> child) => CombineFolder(child.Value).Create();
        
        public override void SyncStructure(FsFolder destination)
        {
            var folders = Folders.ToList();
            var destFolders = destination.Folders.ToList();

            foreach (var subFolder in destFolders.Except(folders))
            {
                destination.CombineFolder(subFolder.Value).Remove();
            }
            
            foreach (var subFile in destination.Files.Except(Files))
            {
                destination.CombineFile(subFile.Value).Remove();
            }

            foreach (var folder in folders.Except(destFolders))
            {
                destination.CombineFolder(folder.Value).Create();
            }
        }
        
        public override void Remove()
        {
            if (!Exists)
            {
                return;
            }

            foreach (var file in Files)
            {
                file.Child.Remove();
            }
            
            Directory.Delete(Value, true);
        }
        
        public override void Create()
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
                Parent.Create();
            }

            Directory.CreateDirectory(Value);
        }
    }
}