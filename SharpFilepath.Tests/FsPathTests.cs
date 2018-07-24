using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Tests
{
    [TestFixture]
    public class FsPathTests
    {
        [Test]
        public void ShouldHandleEqualityOperators()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = Path.Combine(dir).ToPath();
            var sut2 = Path.Combine(dir).ToPath();
            var sut3 = sut.Parent;
            
            Assert.That(sut.Equals(sut2), Is.True);
            
            Assert.That(sut == sut2, Is.True);
            Assert.That(sut == sut3, Is.False);
            Assert.That(sut != sut2, Is.False);
            Assert.That(sut != sut3, Is.True);
        }

        [Test]
        public void ShouldUseGetSize()
        {
            var sut = new Mock<FsPath>("C:\\test.txt");
            sut.Protected().Setup<long>("GetSize").Returns(100);
            
            Assert.That(sut.Object.Size, Is.EqualTo(100));
        }
        
        [Test]
        public void ShouldImplicitlyConvertToString()
        {
            Assert.That(string.Equals("C:\\".ToPath(), "C:\\"), Is.True);
        }

        [Test]
        public void ToStringOverride()
        {
            var sut = "C:\\".ToPath();
            
            Assert.That(sut.ToString(), Is.EqualTo("C:\\"));
        }

        [Test]
        public void GetHashCodeOverride()
        {
            var sut = "C:\\".ToPath();
            
            Assert.That(sut.GetHashCode(), Is.EqualTo("C:\\".GetHashCode()));
        }
    }
}