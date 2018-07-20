using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Tests
{
    [TestFixture]
    public class PathTests
    {
        [Test]
        public void ShouldCreateDirectoryInstance()
        {
            var sut = FsObject.FromString("C:\\");
            
            Assert.That(sut.ToString(), Is.EqualTo("C:\\"));
            Assert.That(sut is Folder);
        }

        [Test]
        public void ShouldCreateFileInstance()
        {
            var path = System.IO.Path.Combine(
                Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName,
                "nunit.framework.dll");
            
            var sut = FsObject.FromString(path);
            
            Assert.That(sut is File);
        }
        
        [Test]
        public void ShouldCreateEmptyInstance()
        {
            var sut = FsObject.FromString("");
            Assert.That(sut is EmptyPath);
            
            sut = FsObject.FromString(null);
            Assert.That(sut is EmptyPath);
        }

        [Test]
        public void ShouldImplicitlyConvertToString()
        {
            var sut = FsObject.FromString("C:\\");
            
            Assert.That(sut == "C:\\");
        }
        
        [Test]
        public void ShouldHandleEqualOperators()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = FsObject.FromString("C:\\");
            var sut2 = FsObject.FromString("C:\\");
            var sut3 = FsObject.FromString(System.IO.Path.Combine(dir));
            
            Assert.That(sut == sut2, Is.True);
            Assert.That(sut == sut3, Is.False);
            Assert.That(sut.Equals(sut2), Is.True);
        }
        
        [Test]
        public void ShouldHandleGreaterOperators()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = FsObject.FromString(System.IO.Path.Combine(dir));
            var sut2 = FsObject.FromString(System.IO.Path.Combine(dir));
            var sut3 = sut.ParentDirectory as FsObject;
            
            Assert.That(sut >= sut2, Is.True);
            Assert.That(sut2 >= sut, Is.True);
            Assert.That(sut3 > sut2, Is.True);
            Assert.That(sut > sut3, Is.False);
            Assert.That(sut < sut3, Is.True);
            Assert.That(sut3 < sut, Is.False);
        }

        [Test]
        public void ShouldCombine()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = FsObject.FromString(dir);
            var parent = sut.ParentDirectory;
            var name = sut.ToString().Split('\\').Last();
            
            Assert.That(parent.Combine(name).ToString(), Is.EqualTo(sut.ToString()));
        }
        
        [Test]
        public void ShouldGetParentDirectory()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var parentDir = Directory.GetParent(dir).FullName;
            var sut = FsObject.FromString(dir);
            var parent = sut.ParentDirectory;
            
            Assert.That(parent.ToString(), Is.EqualTo(parentDir));
        }

        [Test]
        public void ShouldResolveWorkdir()
        {
            var file = System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).Name;
            var parentDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();
            
            var sut = FsObject.FromString($".\\{file}");
            var result = sut.Resolve(parentDir);
            
            Assert.That(result.ToString(), Is.EqualTo(Assembly.GetExecutingAssembly().Location));
        }
        
        [Test]
        public void ShouldResolveWorkdirsParent()
        {
            var file = System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).Name;
            var parentDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();
            var expected = System.IO.Path.Combine(
                Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent?.FullName?? string.Empty,
                file);
            
            var sut = FsObject.FromString($"..\\{file}");
            var result = sut.Resolve(parentDir);
            
            Assert.That(result.ToString(), Is.EqualTo(expected));
        }
    }
}