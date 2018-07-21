using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void ToPathShouldReturnNullForEmptyString()
        {
            Assert.That(string.Empty.ToPath(), Is.Null);
            Assert.That(((string)null).ToPath(), Is.Null);
            Assert.That(" ".ToPath(), Is.Null);
        }
        
        [Test]
        public void ToFolderShouldReturnNullForEmptyString()
        {
            Assert.That(string.Empty.ToFolder(), Is.Null);
            Assert.That(((string)null).ToFolder(), Is.Null);
            Assert.That(" ".ToFolder(), Is.Null);
        }
        
        [Test]
        public void ToFileShouldReturnNullForEmptyString()
        {
            Assert.That(string.Empty.ToFile(), Is.Null);
            Assert.That(((string)null).ToFile(), Is.Null);
            Assert.That(" ".ToFile(), Is.Null);
        }

        [Test]
        public void ShouldReturnFolderPath()
        {
            Assert.That("C:\\Windows".ToPath(), Is.AssignableTo<FsFolder>());
        }
        
        [Test]
        public void ShouldReturnFilePath()
        {
            var filePath = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
            Assert.That(filePath.ToPath(), Is.AssignableTo<FsFile>());
        }
        
        [Test]
        public void ShouldResolveRelativeFolder()
        {
            var folderName = Path.GetFullPath(".").Split('\\').Last();
            Assert.That($"..\\.\\{folderName}".ToFolder(), Is.EqualTo(Path.GetFullPath(".").ToFolder()));
        }
        
        [Test]
        public void ShouldResolveRelativeFile()
        {
            var fileName = Path.GetFileName(Assembly.GetCallingAssembly().Location);
            var fullName = $"{Path.GetFullPath(".")}\\{fileName}";
            
            Assert.That($".\\{fileName}".ToFile(), Is.EqualTo(fullName.ToFile()));
        }
        
        [Test]
        public void ShouldResolveFilePath()
        {
            var fileName = Path.GetFileName(Assembly.GetCallingAssembly().Location);
            var fullName = $"{Path.GetFullPath(".")}\\{fileName}";
            
            Assert.That($".\\{fileName}".ToPath(), Is.EqualTo(fullName.ToFile()));
        }
        
        [Test]
        public void ShouldResolveParentPath()
        {
            var fileName = Path.GetFileName(Assembly.GetCallingAssembly().Location);
            var folderName = Path.GetFullPath(".").Split('\\').Last();
            var fullName = $"{Path.GetFullPath(".")}\\{fileName}";
            
            Assert.That($"..\\{folderName}\\{fileName}".ToPath(), Is.EqualTo(fullName.ToFile()));
        }
    }
}