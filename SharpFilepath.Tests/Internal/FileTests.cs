using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Tests
{
    [TestFixture]
    public class FileTests
    {
        private static FsFolder AppFsFolder => 
            (FsFolder)Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();

        private readonly FsFolder _folder = AppFsFolder.CombineFolder("FileTests");

        [OneTimeSetUp]
        public void Setup()
        {
            _folder.Create();
        }
        
        [OneTimeTearDown]
        public void TearDown()
        {
            _folder.Remove();
        }
        
        [Test]
        public void ShouldCreateInstance()
        {
            var path = "C:\\test.txt";
            var sut = path.ToFile();
            Assert.That(sut.Path, Is.EqualTo(path));
        }

        [Test]
        public void ShouldReturnEncoding()
        {
            var sut = _folder.CombineFile(nameof(ShouldReturnEncoding));
            
            sut.Write("A");
            
            Assert.That(sut.Encoding, Is.EqualTo(Encoding.UTF8));
            File.WriteAllText(sut, "AV", Encoding.ASCII);
            Assert.That(sut.Encoding, Is.EqualTo(Encoding.ASCII));
        }
        
        [Test]
        public void ShouldTestEncoding()
        {
            var sut = _folder.CombineFile(nameof(ShouldTestEncoding));
            sut.Write("A");
            Assert.That(sut.HasEncoding(Encoding.UTF8), Is.True);
            Assert.That(sut.HasEncoding(Encoding.ASCII), Is.False);
            File.WriteAllText(sut, "AV", Encoding.ASCII);
            Assert.That(sut.HasEncoding(Encoding.ASCII), Is.True);
            Assert.That(sut.HasEncoding(Encoding.UTF8), Is.False);
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
        public void ShouldReturnFalseToIsFolder()
        {
            var sut = "C:\\test.txt".ToFile();
            Assert.That(sut.IsFile, Is.True);
            Assert.That(sut.IsFolder, Is.False);
        }
        
        [Test]
        public void ShouldReturnFileName()
        {
            var fileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            var sut = Path.GetFullPath(Assembly.GetExecutingAssembly().Location).ToFile();
            Assert.That(sut.Name, Is.EqualTo(fileName));
        }
        
        [Test]
        public void ShouldReturnFileNameWithoutExtension()
        {
            var fileName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            var sut = Path.GetFullPath(Assembly.GetExecutingAssembly().Location).ToFile();
            Assert.That(sut.NameWithoutExtension, Is.EqualTo(fileName));
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
        public void ShouldCopySmallFileWithoutProgress()
        {
            var sut = _folder.CombineFile("ShouldCopySmallFileWithoutProgress_1.txt");
            
            File.WriteAllText(sut, "ABCD");

            var file = _folder.CombineFile("ShouldCopySmallFileWithoutProgress_2.txt");

            Assert.That(File.Exists(sut));
            
            sut.Copy(file);
            
            Assert.That(File.Exists(file));
            
            Assert.That(sut.Content, Is.EqualTo(file.Content));
        }
        
        [Test]
        public void ShouldCopySmallFileWithProgress()
        {
            var sut = _folder.CombineFile("ShouldCopySmallFileWithoutProgress_1.txt");
            
            File.WriteAllText(sut, "ABCD");

            var file = _folder.CombineFile("ShouldCopySmallFileWithoutProgress_2.txt");

            Assert.That(File.Exists(sut));
            
            var progresses = new List<int>();
            
            sut.Copy(file, i => progresses.Add(i));
            
            Assert.That(File.Exists(file));
            Assert.That(sut.Content, Is.EqualTo(file.Content));
            Assert.That(progresses.SingleOrDefault(), Is.EqualTo(100));
        }
        
        [Test]
        public void ShouldRemoveFile()
        {
            var sut = _folder.CombineFile("ShouldRemoveFile_1.txt");
            File.WriteAllText(sut, "ABCD");
            Assert.That(File.Exists(sut));
            sut.Remove();
            Assert.That(!File.Exists(sut));
        }
        
        [Test]
        public void ShouldGetFilesContent()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.Create();
            File.WriteAllText(folder.CombineFile("test1.txt"), "ABCD");
            
            var sut = folder.CombineFile("test1.txt");
            
            Assert.That(sut.Content, Is.EqualTo("ABCD"));
            
            folder.Remove();
        }
        
        [Test]
        public void ShouldWriteFilesContent()
        {
            var sut = _folder.CombineFile("ShouldWriteFilesContent.txt");
            
            File.WriteAllText(sut, "");
            
            Assert.That(File.ReadAllText(sut).Length, Is.EqualTo(0));
            
            sut.Write("AAAA");
            
            Assert.That(File.ReadAllText(sut), Is.EqualTo("AAAA"));
        }

        [Test]
        public void ShouldCalculateHash()
        {
            var sut = _folder.CombineFile("ShouldCalculateHash.txt");
            
            sut.Write("1");
            
            Assert.That(sut.Hash, Is.EqualTo(new []
            {
                90, 189, 182, 23, 95, 130, 15, 10, 195, 216, 
                100, 127, 187, 31, 122, 11, 204, 145, 117, 122, 
                120, 42, 138, 20, 85, 112, 148, 76, 166, 160, 
                12, 150
            }));
        }
        
        [Test]
        public void ShouldCalculateSize()
        {
            var message = "111";
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.Create();
            
            var sut = folder.CombineFile("test5.txt");
            
            sut.Write(message);
            
            Assert.That(sut.Size, Is.EqualTo(message.Length * 2));
            
            folder.Remove();
        }
        
        [Test]
        public void ShouldRecalculateSize()
        {
            var message = "111";
            
            var sut = _folder.CombineFile(nameof(ShouldRecalculateSize));
            
            sut.Write(message);
            
            Assert.That(sut.Size, Is.EqualTo(message.Length * 2));
            
            sut.Write(message + message);
            
            sut.RefreshSize();
            
            Assert.That(sut.Size, Is.EqualTo(message.Length * 3));
        }
        
        [Test]
        public void ShouldWriteFilesContentWhenFileDoesNotExist()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.Create();
            
            var sut = folder.CombineFile("test3.txt");
            
            sut.Write("AAAA");
            
            Assert.That(File.ReadAllText(sut), Is.EqualTo("AAAA"));
            
            folder.Remove();
        }
        
        [Test]
        public void ShouldRemoveReadOnlyFile()
        {
            var sut = _folder.CombineFile(nameof(ShouldRemoveReadOnlyFile));
            File.WriteAllText(sut, "ABCD");
            File.SetAttributes(sut, FileAttributes.ReadOnly);
            Assert.That(File.Exists(sut));
            sut.Remove();
            Assert.That(!File.Exists(sut));
        }
        
        [Test]
        public void ShouldThrowWhenCantRemoveFile()
        {
            var target = AppFsFolder.CombineFile("RoseByte.SharpFiles.Tests.dll");
            
            Assert.That(
                () => target.Remove(), 
                Throws.Exception.With.Message.Contains(
                    $"is locked by: "));
        }
        
        [Test]
        public void ShouldThrowOnCopyingWithoutProgressOverLockedFile()
        {
            var sut = AppFsFolder.CombineFile("TestFiles\\nuget.exe");
            var target = AppFsFolder.CombineFile("RoseByte.SharpFiles.Tests.dll");
            
            Assert.That(
                () => sut.Copy(target), 
                Throws.Exception.With.Message.StartsWith($"File '{sut}' could not be copied to '{target}': "));
        }
    }
}