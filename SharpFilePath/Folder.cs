﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpFilePath
{
    public class Folder : SharpPath, IFolder
    {
        public Folder(string value) : base(value) { }
        
        public override bool Exists => Directory.Exists(Value);

        public override void Remove()
        {
            Directory.Delete(Value, true);
        }

        public override void Backup(SharpPath where, Action<int> progress)
        {
            throw new NotImplementedException();
        }

        public override void Restore(SharpPath from, Action<int> progress)
        {
            throw new NotImplementedException();
        }

        public override void Copy(SharpPath target, Action<int> progress)
        {
            
            
        }

        public void Mirror(IPath target)
        {
            throw new NotImplementedException();
        }

        public void SyncStructure(IFolder destination)
        {
            destination.GetFolders().Except(GetFolders()).ToList().ForEach(x => x.Child.Remove());
            destination.GetFiles().Except(GetFiles()).ToList().ForEach(x => x.Child.Remove());
            GetFolders().ToList().ForEach(x => x.Child.EnsureDirectoryCreated());
        }

        public IEnumerable<SubPath<IFile>> NewerFilesThan(IFolder destination)
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
        
        public IEnumerable<SubPath<IFile>> GetFiles(bool recursive = true, string mask = null, IEnumerable<string> exceptions = null)
        {
            var results = Directory.EnumerateFiles(
                Value,
                mask ?? "*",
                recursive ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
            
            if (exceptions != null)
            {
                results = results.Where(x => !exceptions.Any(x.StartsWith)).ToList();
            }

            return results.Select(x => new SubPath<IFile>(this, new File(x)));
        }

        public IEnumerable<SubPath<IFolder>> GetFolders(bool recursive = true, string mask = null, IEnumerable<string> exceptions = null)
        {
            var results = Directory.EnumerateDirectories(
                Value,
                mask ?? "*",
                recursive ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
            
            if (exceptions != null)
            {
                results = results.Where(x => !exceptions.Any(x.StartsWith)).ToList();
            }

            return results.Select(x => new SubPath<IFolder>(this, new Folder(x)));
        }

        public virtual void EnsureDirectoryCreated()
        {
            if (Exists)
            {
                return;
            }
            
            if (!ParentDirectory.Exists)
            {
                ParentDirectory.EnsureDirectoryCreated();
            }

            Directory.CreateDirectory(Value);
        }
    }
}