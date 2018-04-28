using NUnit.Framework;

namespace RoseByte.SharpFiles.Tests
{
    [TestFixture]
    public class SubPathTests
    {
        [Test]
        public void ShouldCreateInstance()
        {
            var sut = new SubPath<Folder>(new Folder("C:"), new Folder("C:\\SomeFolder"));
            
            Assert.That(sut.Child.ToString(), Is.EqualTo("C:\\SomeFolder"));
            Assert.That(sut.Parent.ToString(), Is.EqualTo("C:"));
            Assert.That(sut.Value, Is.EqualTo("SomeFolder"));
        }
    }
}