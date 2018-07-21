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
        public void ShouldHandleEqualOperators()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = "C:\\".ToPath();
            var sut2 = "C:\\".ToPath();
            var sut3 = Path.Combine(dir).ToPath();
            
            Assert.That(sut == sut2, Is.True);
            Assert.That(sut == sut3, Is.False);
            Assert.That(sut.Equals(sut2), Is.True);
        }
        
        [Test]
        public void ShouldHandleGreaterOperators()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = Path.Combine(dir).ToPath();
            var sut2 = Path.Combine(dir).ToPath();
            var sut3 = sut.Parent;
            
            Assert.That(sut >= sut2, Is.True);
            Assert.That(sut2 >= sut, Is.True);
            Assert.That(sut3 > sut2, Is.True);
            Assert.That(sut > sut3, Is.False);
            Assert.That(sut < sut3, Is.True);
            Assert.That(sut3 < sut, Is.False);
        }

        
    }
}