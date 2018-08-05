using System.IO;
using System.Reflection;
using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Tests
{
    [TestFixture]
    public class PathTests
    {
        private static FsFolder AppFsFolder => 
            (FsFolder)Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();

        private readonly FsFolder _folder = AppFsFolder.CombineFolder(nameof(PathTests));

        [OneTimeSetUp]
        public void Setup() => _folder.Create();
        
        [OneTimeTearDown]
        public void TearDown() => _folder.Remove();

        [Test]
        public void ShouldReturnFalseToFIleAndFolderTests()
        {
            var sut = "C:\\notHere.txt".ToPath();
            
            Assert.That(sut.IsFile, Is.False);
            Assert.That(sut.IsFolder, Is.False);
        }
        
        [Test]
        public void ShouldReflectIfExists()
        {
            var folder = _folder.CombineFolder(nameof(ShouldReflectIfExists));
            folder.Create();
            var file = folder.CombineFile("exists.txt");

            var sut = file.ToString().ToPath();
            
            Assert.That(sut.Exists, Is.False);
            
            file.Write("A");
            
            Assert.That(sut.Exists, Is.True);
        }
    }
}