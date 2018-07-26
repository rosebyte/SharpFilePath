using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Tests
{
    [TestFixture]
    public class FilteredFolderTests
    {
        private static FsFolder AppFsFolder => 
            (FsFolder)Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();

        private readonly FsFolder _folder = AppFsFolder.CombineFolder(nameof(FilteredFolderTests));

        [OneTimeSetUp]
        public void Setup()
        {
            _folder.Create();

            _folder.CombineFile("Test_0_1.txt").Write("0_1");
            _folder.CombineFile("Test_0_2.txt").Write("0_2");
            
            var subfolder1 = _folder.CombineFolder("SubFolder_1");
            subfolder1.Create();
            subfolder1.CombineFile("Test_1_1.txt").Write("1_1");
            subfolder1.CombineFile("Test_1_2.txt").Write("1_2");
            
            var subfolder11 = _folder.CombineFolder("SubFolder_1_1");
            subfolder11.Create();
            subfolder11.CombineFile("Test_1_1_1.txt").Write("1_1_1");
            subfolder11.CombineFile("Test_1_1_2.txt").Write("1_1_2");
            
            var subfolder12 = _folder.CombineFolder("SubFolder_1_2");
            subfolder12.Create();
            subfolder12.CombineFile("Test_1_2_1.txt").Write("1_2_1");
            subfolder12.CombineFile("Test_1_2_2.txt").Write("1_2_2");
            
            var subfolder2 = _folder.CombineFolder("SubFolder_2");
            subfolder2.Create();
            subfolder2.CombineFile("Test_2_1.txt").Write("2_1");
            subfolder2.CombineFile("Test_2_2.txt").Write("2_2");
            
            var subfolder21 = _folder.CombineFolder("SubFolder_2_1");
            subfolder21.Create();
            subfolder21.CombineFile("Test_2_1_1.txt").Write("2_1_1");
            subfolder21.CombineFile("Test_2_1_2.txt").Write("2_1_2");
            
            var subfolder22 = _folder.CombineFolder("SubFolder_2_2");
            subfolder22.Create();
            subfolder22.CombineFile("Test_2_2_1.txt").Write("2_2_1");
            subfolder22.CombineFile("Test_2_2_2.txt").Write("2_2_2");
        }
        
        [OneTimeTearDown]
        public void TearDown() => _folder.Remove();

        [Test]
        public void ShouldReturnAllFiles()
        {
            var sut = _folder.Filter(true, null, null, null, null);
            
            Assert.That(
                sut.Files.Count(), 
                Is.EqualTo(Directory.EnumerateFiles(_folder, "*", SearchOption.AllDirectories).Count()));
        }
        
        [Test]
        public void ShouldReturnAllFolders()
        {
            var sut = _folder.Filter(true, null, null, null, null);
            
            Assert.That(
                sut.Folders.Count(), 
                Is.EqualTo(Directory.EnumerateDirectories(_folder, "*", SearchOption.AllDirectories).Count()));
        }

        [Test]
        public void ShouldFilterToFilesEndingWIthOne()
        {
            var sut = _folder.Filter(true, null, null, new Regex(".*1\\.txt"), null);

            var result = sut.Files;
            
            Assert.That(result.Count(), Is.EqualTo(7));
        }
        
        [Test]
        public void ShouldSkipFilesEndingWithOne()
        {
            var sut = _folder.Filter(true, null, null, null, new Regex(".*1\\.txt"));

            var result = sut.Files;
            
            Assert.That(result.Count(), Is.EqualTo(7));
        }
    }
}