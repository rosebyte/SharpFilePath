using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Tests
{
    [TestFixture]
    public class FileTests
    {
        private static Folder AppFolder => 
            (Folder)Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();
        
        [Test]
        public void ShouldCopySmallFile()
        {
            var progresses = new List<(long, long, long)>();
            
            var folder = (Folder)AppFolder.Combine("CopyTest");
            folder.CreateIfNotExists();
            System.IO.File.WriteAllText(folder.Combine("test1.txt").ToString(), "ABCD");

            var sut = folder.Combine("test1.txt") as File;
            
            Assert.That(sut, Is.Not.Null);
            
            sut.Copy(folder.Combine("test2.txt"), (a, b, c) => progresses.Add((a, b, c)));
            
            Assert.That(progresses.Count, Is.EqualTo(1));
            Assert.That(progresses[0].Item1, Is.EqualTo(4));
            Assert.That(progresses[0].Item2, Is.EqualTo(4));
            Assert.That(progresses[0].Item3, Is.EqualTo(4));
            
            Assert.That(System.IO.File.Exists(folder.Combine("test2.txt").ToString()));
            
            Assert.That(
                ((File)folder.Combine("test1.txt")).Content, 
                Is.EqualTo(((File)folder.Combine("test2.txt")).Content));
            
            folder.Remove();
        }
        
        [Test]
        public void ShouldCopyLargeFile()
        {
            var progresses = new List<(long, long, long)>();
            
            var sut = (File)AppFolder.Combine("TestFiles\\nuget.exe");
            var target = (File)AppFolder.Combine("TestFiles\\nuget2.exe");
            
            sut.Copy(target, (a, b, c) => progresses.Add((a, b, c)));
            
            Assert.That(progresses.Count, Is.GreaterThan(1));
            for (var i = 1; i<progresses.Count;i++)
            {
                Assert.That(progresses[i].Item2 - progresses[i].Item1, Is.EqualTo(progresses[i - 1].Item2));
                Assert.That(progresses[i].Item2 - progresses[i - 1].Item2, Is.EqualTo(progresses[i].Item1));
                Assert.That(progresses[i].Item3, Is.EqualTo(progresses[i - 1].Item3));
            }

            target.Remove();
        }
    }
}