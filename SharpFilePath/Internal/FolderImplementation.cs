using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Internal
{
    public class FolderImplementation : Folder
    {
        internal FolderImplementation(string value) : base(value) { }
        
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

        public override void Copy(FsObject target)
        {
            throw new NotImplementedException();
        }

        public override void Copy(FsObject target, Action<int> progress)
        {
            throw new NotImplementedException();
        }

        public override void Copy(Folder target)
        {
            GetFiles().ToList().ForEach(x => x.Child.Copy(target.Combine(x.Value)));
        }
        
        public override FsObject Combine(string pathPart) => Path.Combine(this, pathPart).ToPath();
        
        public override Folder GetSubFolder(string pathPart) => new FolderImplementation(Path.Combine(Value, pathPart));
        
        public override void Copy(Folder target, Predicate<SubPath<File>> predicate)
        {
            GetFiles().Where(x => predicate(x)).ToList().ForEach(x => x.Child.Copy(target.Combine(x.Value)));
        }

        public override void Copy(List<Regex> skip, Folder target, Action<int> progress = null)
        {
            foreach (var folder in target.GetFolders(true, null, skip).Except(GetFolders(true, null, skip)))
            {
                folder.Child.CreateIfNotExists();
            }

            foreach (var file in GetFiles(true, null, skip))
            {
                file.Child.Copy(target.Combine(file.Value), progress);
            }
        }

        public override FsObject Resolve() => Resolve(new FolderImplementation(Environment.CurrentDirectory));

        public override FsObject Resolve(Folder pwd)
        {
            if (pwd == null)
            {
                throw new Exception($"Working directory demanded and not set: {Value}");
            }
			
            if (Value.StartsWith(".\\") || Value.StartsWith("./"))
            {
                return new FolderImplementation(Path.Combine(pwd, Value.Substring(2)));
            }

            if (Value.StartsWith("..\\") || Value.StartsWith("../"))
            {
                return pwd.Parent.Combine(Value.Substring(3));
            }

            return this;
        }

        public override Folder Parent => new FolderImplementation(Directory.GetParent(Value).FullName);

        public override void SyncStructure(Folder destination, IList<Regex> exceptions = null, Action<int> progress = null)
        {
            var folders = GetFolders(true, null, exceptions).ToList();
            var destFolders = destination.GetFolders(true, null, exceptions).ToList();

            progress?.Invoke(5);
            
            foreach (var folder in destFolders.Except(folders))
            {
                folder.Child.Remove();
            }

            progress?.Invoke(10);

            var files = GetFiles(true, null, exceptions).ToList();
            var destFiles = destination.GetFiles(true, null, exceptions).ToList();

            progress?.Invoke(15);
            
            foreach (var file in destFiles.Except(files))
            {
                file.Child.Remove();
            }

            progress?.Invoke(20);

            foreach (var folder in folders.Except(destFolders))
            {
                folder.Child.CreateIfNotExists();
            }

            progress?.Invoke(25);

            var total = files.Count;

            for (var i = 0; i < total; i++ )
            {
                files[i].Copy(destination);
                progress?.Invoke(75 * i / total);
            }
        }

        public override IEnumerable<SubPath<File>> GetFiles(bool recursive = true, string mask = null, 
            IList<Regex> exceptions = null)
        {
            var helper = new ScanHelper(this);

            foreach (var file in helper.GetFiles(Value, recursive, mask ?? "*", exceptions ?? new List<Regex>()))
            {
                yield return file;
            }
        }

        public override IEnumerable<SubPath<Folder>> GetFolders(
            bool recursive = true, string mask = null, IList<Regex> exceptions = null)
        {
            var results = Directory.EnumerateDirectories(
                Value,
                mask ?? "*",
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Select(x => new SubPathImplementation<Folder>(this, new FolderImplementation(x)))
                .ToList();

            if (exceptions != null && exceptions.Any())
            {
                return results.Where(x => !exceptions.Any(y => y.IsMatch(x.Value)));
                
            }
            
            return results;
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