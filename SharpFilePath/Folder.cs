using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RoseByte.SharpFiles.Extensions;
using RoseByte.SharpFiles.Interfaces;

namespace RoseByte.SharpFiles
{
    public class Folder : Path, IFolder
    {
        public Folder(string value) : base(value) { }
        
        public override bool Exists => Directory.Exists(Value);

        public override void Remove()
        {
            Directory.Delete(Value, true);
        }

        public override void Copy(IPath target, Action<long, long, long> progress)
        {
            if (!(target is IFolder dest))
            {
                throw new Exception("Folder can be copied only to folder.");
            }
            
            NewerFilesThan(dest).ToList().ForEach(x => x.Child.Copy(dest.Combine(x.Value), progress));
        }

        public void Mirror(IFolder target, Action<long, long, long> progress, IEnumerable<string> exceptions = null)
        {
            SyncStructure(target, exceptions);
            NewerFilesThan(target).ToList().ForEach(x => x.Child.Copy(target.Combine(x.Value), progress));
        }

        public void SyncStructure(IFolder destination, IEnumerable<string> exceptions = null)
        {
            var skips = exceptions?.ToList() ?? new List<string>();
            
            destination.GetFolders(true, null, skips).Except(GetFolders()).ToList().ForEach(x => x.Child.Remove());
            destination.GetFiles().Except(GetFiles(true, null, skips)).ToList().ForEach(x => x.Child.Remove());
            GetFolders().ToList().ForEach(x => (destination.GetSubFolder(x.Value)).CreateIfNotExists());
        }

        public IEnumerable<ISubPath<IFile>> NewerFilesThan(IFolder destination)
        {
            var dest = destination.GetFiles().ToDictionary(x => x.ToString(), x => x.Child);
            
            foreach (var file in GetFiles())
            {
                if (!dest.TryGetValue(file.Value, out var doppelGanger))
                {
                    yield return file;
                    continue;
                }

                if (doppelGanger.Size != file.Child.Size || doppelGanger.Changed != file.Child.Changed)
                {
                    yield return file;
                }
            }
        }
        
        public IEnumerable<ISubPath<IFile>> GetFiles(bool recursive = true, string mask = null, IEnumerable<string> exceptions = null)
        {
            var results = Directory.EnumerateFiles(
                Value,
                mask ?? "*",
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            
            if (exceptions != null)
            {
                results = results.Where(x => !exceptions.Any(x.StartsWith)).ToList();
            }

            return results.Select(x => new SubPath<IFile>(this, new File(x)));
        }

        public IEnumerable<ISubPath<IFolder>> GetFolders(bool recursive = true, string mask = null, IEnumerable<string> exceptions = null)
        {
            var results = Directory.EnumerateDirectories(
                Value,
                mask ?? "*",
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            
            if (exceptions != null)
            {
                results = results.Where(x => !exceptions.Any(x.StartsWith)).ToList();
            }

            return results.Select(x => new SubPath<IFolder>(this, new Folder(x)));
        }

        public void CreateIfNotExists()
        {
            if (Exists)
            {
                return;
            }

            if (ParentDirectory == null)
            {
                throw new Exception("Top level node (e.g. drive) can't be created.");
            }
            
            if (!ParentDirectory.Exists)
            {
                ParentDirectory.CreateIfNotExists();
            }

            Directory.CreateDirectory(Value);
        }
    }
}