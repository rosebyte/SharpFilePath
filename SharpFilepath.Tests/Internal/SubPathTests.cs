using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;
using RoseByte.SharpFiles.Internal;

namespace RoseByte.SharpFiles.Tests
{
    [TestFixture]
    public class SubPathTests
    {
        [Test]
        public void ShouldCreateInstance()
        {
            var sut = new SubPath<FsFolder>("C:".ToFolder(), "C:\\SomeFolder".ToFolder());
            
            Assert.That(sut.Child.ToString(), Is.EqualTo("C:\\SomeFolder"));
            Assert.That(sut.Parent.ToString(), Is.EqualTo("C:"));
            Assert.That(sut.Value, Is.EqualTo("SomeFolder"));
        }
    }
}