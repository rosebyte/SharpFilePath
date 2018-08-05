using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;
using RoseByte.SharpFiles.Internal;

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
        public void ShouldReturnPathIfPathDoesNotExist()
        {
            var sut = "C:\\test.txt".ToPath();
            
            Assert.That(sut, Is.TypeOf<Path>());
        }
        
        [Test]
        public void ShouldReturnFilePath()
        {
            var filePath = System.IO.Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
            Assert.That(filePath.ToPath(), Is.AssignableTo<FsFile>());
        }
        
        [Test]
        public void ShouldResolveRelativeFolder()
        {
            var folderName = System.IO.Path.GetFullPath(".").Split('\\').Last();
            Assert.That($"..\\.\\{folderName}".ToFolder(), Is.EqualTo(System.IO.Path.GetFullPath(".").ToFolder()));
        }
        
        [Test]
        public void ShouldResolveRelativeFile()
        {
            var fileName = System.IO.Path.GetFileName(Assembly.GetCallingAssembly().Location);
            var fullName = $"{System.IO.Path.GetFullPath(".")}\\{fileName}";
            
            Assert.That($".\\{fileName}".ToFile(), Is.EqualTo(fullName.ToFile()));
        }
        
        [Test]
        public void ShouldResolveFilePath()
        {
            var fileName = System.IO.Path.GetFileName(Assembly.GetCallingAssembly().Location);
            var fullName = $"{System.IO.Path.GetFullPath(".")}\\{fileName}";
            
            Assert.That($".\\{fileName}".ToPath(), Is.EqualTo(fullName.ToFile()));
        }
        
        [Test]
        public void ShouldResolveParentPath()
        {
            var fileName = System.IO.Path.GetFileName(Assembly.GetCallingAssembly().Location);
            var folderName = System.IO.Path.GetFullPath(".").Split('\\').Last();
            var fullName = $"{System.IO.Path.GetFullPath(".")}\\{fileName}";
            
            Assert.That($"..\\{folderName}\\{fileName}".ToPath(), Is.EqualTo(fullName.ToFile()));
        }
    }
}