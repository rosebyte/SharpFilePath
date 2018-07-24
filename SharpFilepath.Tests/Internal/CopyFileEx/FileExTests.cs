using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RoseByte.SharpFiles.CopyFileEx;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Tests.CopyFileEx
{
    [TestFixture]
    public class FileExTests
    {
        private static FsFolder AppFsFolder => 
            (FsFolder)Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();

        private readonly FsFolder _folder = AppFsFolder.CombineFolder("CopyFileExTests");

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
        public void ShouldCopySmallFile()
        {
            var progresses = new List<(long, long)>();

            var source = _folder.CombineFile("test1.txt");
            
            File.WriteAllText(source, "ABCD");
            
            var sut = new FileEx((x, y) => progresses.Add((x, y)));
            
            sut.Copy(_folder.CombineFile("test1.txt"), _folder.CombineFile("test2.txt"));
            
            Assert.That(progresses.SingleOrDefault().Item1, Is.EqualTo(progresses.SingleOrDefault().Item2));
            Assert.That(progresses.SingleOrDefault().Item1, Is.EqualTo(source.Size));
            
            Assert.That(File.Exists(_folder.CombineFile("test2.txt").ToString()));
            
            Assert.That(
                _folder.CombineFile("test1.txt").Content, 
                Is.EqualTo(_folder.CombineFile("test2.txt").Content));
        }
        
        [Test]
        public void ShouldCopyLargeFile()
        {
            var progresses = new List<(long, long)>();
            
            var file = AppFsFolder.CombineFile("TestFiles\\nuget.exe");
            var target = _folder.CombineFile("test3.exe");
            
            var sut = new FileEx((x, y) => progresses.Add((x, y)));
            
            sut.Copy(file, target);
            
            Assert.That(progresses.Count, Is.GreaterThan(1));
            
            Assert.That(progresses.First().Item1, Is.EqualTo(progresses.First().Item2));
            
            for (var i = 1; i < progresses.Count; i++)
            {
                Assert.That(
                    progresses[i].Item2, 
                    Is.EqualTo(progresses[i - 1].Item2 + progresses[i].Item1));
            }
            
            Assert.That(progresses.Last().Item2, Is.EqualTo(file.Size));
        }
        
        [Test]
        public void ShouldThrowOnCopyingOverLockedFile()
        {
            var sut = AppFsFolder.CombineFile("TestFiles\\nuget.exe");
            var target = AppFsFolder.CombineFile("RoseByte.SharpFiles.Tests.dll");
            
            Assert.That(
                () => sut.Copy(target, (x, y) => { }), 
                Throws.Exception.With.Message.EqualTo($"File '{sut}' could not be copied to '{target}'"));
        }
    }
}