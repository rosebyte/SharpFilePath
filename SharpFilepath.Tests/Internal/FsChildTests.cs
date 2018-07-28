using System.IO;
using System.Reflection;
using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;
using RoseByte.SharpFiles.Internal;

namespace RoseByte.SharpFiles.Tests
{
    [TestFixture]
    public class FsChildTests
    {
        [Test]
        public void ShouldCreateInstance()
        {
            var sut = new FsChild<FsFolder>("C:\\".ToFolder(), "C:\\SomeFolder".ToFolder());
            
            Assert.That(sut.Child.ToString(), Is.EqualTo("C:\\SomeFolder"));
            Assert.That(sut.Parent.ToString(), Is.EqualTo("C:"));
            Assert.That(sut.SubPath, Is.EqualTo("SomeFolder"));
        }

        [Test]
        public void ShouldPassPathTypeForFile()
        {
            var sut = new FsChild<FsFile>("C:\\".ToFolder(), "C:\\Test.txt".ToFile());
            
            Assert.That(sut.IsFile, Is.True);
            Assert.That(sut.IsFolder, Is.False);
        }
        
        [Test]
        public void ShouldPassPathTypeForFolder()
        {
            var sut = new FsChild<FsFolder>("C:\\".ToFolder(), "C:\\Windows".ToFolder());
            
            Assert.That(sut.IsFile, Is.False);
            Assert.That(sut.IsFolder, Is.True);
        }

        [Test]
        public void ShouldEqualBySubPath()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToFolder();
            var sut = new FsChild<FsFile>(dir, dir.CombineFile("test1.txt"));
            var sut2 = new FsChild<FsFile>(dir, dir.CombineFile("test1.txt"));
            var sut3 = new FsChild<FsFile>(dir, dir.CombineFile("test2.txt"));
            
            Assert.That(sut.Equals(sut2), Is.True);
            
            Assert.That(sut == sut2, Is.True);
            Assert.That(sut == sut3, Is.False);
            Assert.That(sut != sut2, Is.False);
            Assert.That(sut != sut3, Is.True);
        }
        
        [Test]
        public void ToStringOverride()
        {
            var sut = new FsChild<FsFile>("C:\\".ToFolder(), "C:\\TestFolder\\Test.txt".ToFile());
            
            Assert.That(sut.ToString(), Is.EqualTo("TestFolder\\Test.txt"));
        }

        [Test]
        public void GetHashCodeOverride()
        {
            var sut = new FsChild<FsFile>("C:\\".ToFolder(), "C:\\TestFolder\\Test.txt".ToFile());
            
            Assert.That(sut.GetHashCode(), Is.EqualTo("TestFolder\\Test.txt".GetHashCode()));
        }
    }
}