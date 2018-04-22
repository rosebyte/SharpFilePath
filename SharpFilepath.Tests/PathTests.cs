using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SharpFilePath;
using File = SharpFilePath.File;
using Path = SharpFilePath.Path;

namespace SharpFilepath.Tests
{
    [TestFixture]
    public class PathTests
    {
        [Test]
        public void ShouldCreateDirectoryInstance()
        {
            var sut = Path.FromString("C:\\");
            
            Assert.That(sut.ToString(), Is.EqualTo("C:\\"));
            Assert.That(sut is Folder);
        }

        [Test]
        public void ShouldCreateFileInstance()
        {
            var path = System.IO.Path.Combine(
                Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName,
                "nunit.framework.dll");
            
            var sut = Path.FromString(path);
            
            Assert.That(sut is File);
        }
        
        [Test]
        public void ShouldCreateEmptyInstance()
        {
            var sut = Path.FromString("");
            Assert.That(sut is EmptyPath);
            
            sut = Path.FromString(null);
            Assert.That(sut is EmptyPath);
        }

        [Test]
        public void ShouldImplicitlyConvertToString()
        {
            var sut = Path.FromString("C:\\");
            
            Assert.That(sut == "C:\\");
        }
        
        [Test]
        public void ShouldHandleEqualOperators()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = Path.FromString("C:\\");
            var sut2 = Path.FromString("C:\\");
            var sut3 = Path.FromString(System.IO.Path.Combine(dir));
            
            Assert.That(sut == sut2, Is.True);
            Assert.That(sut == sut3, Is.False);
            Assert.That(sut.Equals(sut2), Is.True);
        }
        
        [Test]
        public void ShouldHandleGreaterOperators()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = Path.FromString(System.IO.Path.Combine(dir));
            var sut2 = Path.FromString(System.IO.Path.Combine(dir));
            var sut3 = sut.ParentDirectory as Path;
            
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
            var sut = Path.FromString(dir);
            var parent = sut.ParentDirectory;
            var name = sut.ToString().Split('\\').Last();
            
            Assert.That(parent.Combine(name), Is.EqualTo(sut));
        }
        
        [Test]
        public void ShouldGetParentDirectory()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var parentDir = Directory.GetParent(dir).FullName;
            var sut = Path.FromString(dir);
            var parent = sut.ParentDirectory;
            
            Assert.That(parent.ToString(), Is.EqualTo(parentDir));
        }

        [Test]
        public void ShouldResolveWorkdir()
        {
            var file = System.IO.Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).Name;
            var parentDir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();
            
            var sut = Path.FromString($".\\{file}");
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
            
            var sut = Path.FromString($"..\\{file}");
            var result = sut.Resolve(parentDir);
            
            Assert.That(result.ToString(), Is.EqualTo(expected));
        }
    }
}