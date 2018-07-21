using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Tests
{
    [TestFixture]
    public class FileTests
    {
        private static FsFolder AppFsFolder => 
            (FsFolder)Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();
        
        [Test]
        public void ShouldCreateInstance()
        {
            var path = "C:\\test.txt";
            var sut = path.ToFile();
            Assert.That(sut.Value, Is.EqualTo(path));
        }
        
        [Test]
        public void ShouldTestIfFileExists()
        {
            var sut = "C:\\test_not_here.txt".ToFile();
            Assert.That(sut.Exists, Is.False);

            sut = Path.GetFullPath(Assembly.GetExecutingAssembly().Location).ToFile();
            Assert.That(sut.Exists, Is.True);
        }
        
        [Test]
        public void ShouldImplicitlyConvertToString()
        {
            var sut = "C:\\test.txt".ToFile();
            Assert.That(sut == "C:\\test.txt");
        }
        
        [Test]
        public void ShouldReturnFalseToIsFolder()
        {
            var sut = "C:\\test.txt".ToFile();
            Assert.That(sut.IsFile, Is.True);
            Assert.That(sut.IsFolder, Is.False);
        }
        
        [Test]
        public void ShouldReturnFileName()
        {
            var sut = Path.GetFullPath(Assembly.GetExecutingAssembly().Location).ToFile();
            Assert.That(sut.Name, Is.EqualTo(""));
        }
        
        [Test]
        public void ShouldReturnFileNameWithoutExtension()
        {
            var sut = Path.GetFullPath(Assembly.GetExecutingAssembly().Location).ToFile();
            Assert.That(sut.NameWithoutExtension, Is.EqualTo(""));
        }
        
        [Test]
        public void ShouldGetParentDirectory()
        {
            var file = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var parentDir = Directory.GetParent(file).FullName;
            var sut = Path.Combine(file).ToFile();
            var parent = sut.Parent;
            
            Assert.That(parent.ToString(), Is.EqualTo(parentDir));
        }
        
        [Test]
        public void ShouldCopySmallFile()
        {
            var progresses = new List<int>();
            
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.CreateIfNotExists();
            File.WriteAllText(folder.CombineFile("test1.txt").ToString(), "ABCD");

            var sut = folder.CombineFile("test1.txt");
            
            sut.Copy(folder.CombineFile("test2.txt"), x => progresses.Add(x));
            
            Assert.That(progresses.SingleOrDefault(), Is.EqualTo(100));
            
            Assert.That(File.Exists(folder.CombineFile("test2.txt").ToString()));
            
            Assert.That(
                folder.CombineFile("test1.txt").Content, 
                Is.EqualTo(folder.CombineFile("test2.txt").Content));
            
            folder.Remove();
        }
        
        [Test]
        public void ShouldCopySmallFileWithoutProgress()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.CreateIfNotExists();
            File.WriteAllText(folder.CombineFile("test1.txt"), "ABCD");

            var sut = folder.CombineFile("test1.txt");
            
            folder.CombineFile("test2.txt").Remove();
            
            Assert.That(File.Exists(folder.CombineFile("test2.txt")));
            
            sut.Copy(folder.CombineFile("test2.txt"));
            
            Assert.That(File.Exists(folder.CombineFile("test2.txt")));
            
            Assert.That(
                folder.CombineFile("test1.txt").Content, 
                Is.EqualTo(folder.CombineFile("test2.txt").Content));
            
            folder.Remove();
        }
        
        [Test]
        public void ShouldRemoveFile()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.CreateIfNotExists();
            File.WriteAllText(folder.CombineFile("test1.txt"), "ABCD");

            var sut = folder.CombineFile("test1.txt");
            
            Assert.That(File.Exists(folder.CombineFile("test2.txt")));
            
            sut.Remove();
            
            Assert.That(!File.Exists(folder.CombineFile("test2.txt")));
            
            folder.Remove();
        }
        
        [Test]
        public void ShouldGetFilesContent()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.CreateIfNotExists();
            File.WriteAllText(folder.CombineFile("test1.txt"), "ABCD");
            
            var sut = folder.CombineFile("test1.txt");
            
            Assert.That(sut.Content, Is.EqualTo("ABCD"));
            
            folder.Remove();
        }
        
        [Test]
        public void ShouldWriteFilesContent()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.CreateIfNotExists();
            File.WriteAllText(folder.CombineFile("test1.txt"), "");
            
            var sut = folder.CombineFile("test1.txt");
            
            sut.Write("AAAA");
            
            Assert.That(File.ReadAllText(sut), Is.EqualTo("AAAA"));
            
            folder.Remove();
        }

        [Test]
        public void ShouldCalculateHash()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.CreateIfNotExists();
            
            var sut = folder.CombineFile("test4.txt");
            
            sut.Write("1");
            
            Assert.That(sut.Hash, Is.EqualTo("AAAA"));
            
            folder.Remove();
        }
        
        [Test]
        public void ShouldCalculateSize()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.CreateIfNotExists();
            
            var sut = folder.CombineFile("test5.txt");
            
            sut.Write("111");
            
            Assert.That(sut.Size, Is.EqualTo(3));
            
            folder.Remove();
        }
        
        [Test]
        public void ShouldWriteFilesContentWhenFileDoesNotExist()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.CreateIfNotExists();
            
            var sut = folder.CombineFile("test3.txt");
            
            sut.Write("AAAA");
            
            Assert.That(File.ReadAllText(sut), Is.EqualTo("AAAA"));
            
            folder.Remove();
        }
        
        [Test]
        public void ShouldRemoveReadOnlyFile()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.CreateIfNotExists();
            File.WriteAllText(folder.CombineFile("test1.txt"), "ABCD");

            var sut = folder.CombineFile("test1.txt");
            
            File.SetAttributes(sut, FileAttributes.ReadOnly);
            
            Assert.That(File.Exists(folder.CombineFile("test2.txt")));
            
            sut.Remove();
            
            Assert.That(!File.Exists(folder.CombineFile("test2.txt")));
            
            folder.Remove();
        }
        
        [Test]
        public void ShouldThrowWhenCantRemoveFile()
        {
            var sut = AppFsFolder.CombineFile("TestFiles\\nuget.exe");
            var target = AppFsFolder.CombineFile("RoseByte.SharpFiles.Tests.dll");
            
            Assert.That(
                () => target.Remove(), 
                Throws.Exception.With.Message.EqualTo($"File could not be removed: {sut}"));
        }
        
        [Test]
        public void ShouldCopyLargeFile()
        {
            var progresses = new List<int>();
            
            var sut = AppFsFolder.CombineFile("TestFiles\\nuget.exe");
            var target = AppFsFolder.CombineFile("TestFiles\\nuget2.exe");
            
            sut.Copy(target, x => progresses.Add(x));
            
            Assert.That(progresses.Count, Is.GreaterThan(1));
            Assert.That(progresses.Last(), Is.EqualTo(100));
            target.Remove();
        }
        
        [Test]
        public void ShouldThrowOnCopyingWithoutProgressOverLockedFile()
        {
            var sut = AppFsFolder.CombineFile("TestFiles\\nuget.exe");
            var target = AppFsFolder.CombineFile("RoseByte.SharpFiles.Tests.dll");
            
            Assert.That(
                () => sut.Copy(target), 
                Throws.Exception.With.Message.EqualTo($"File '{sut}' could not be copied to '{target}'"));
        }
        
        [Test]
        public void ShouldThrowOnCopyingOverLockedFile()
        {
            var sut = AppFsFolder.CombineFile("TestFiles\\nuget.exe");
            var target = AppFsFolder.CombineFile("RoseByte.SharpFiles.Tests.dll");
            
            Assert.That(
                () => sut.Copy(target, x => {}), 
                Throws.Exception.With.Message.EqualTo($"File '{sut}' could not be copied to '{target}'"));
        }
    }
}