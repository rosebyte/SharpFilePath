using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;

namespace RoseByte.SharpFiles.Tests.TestFiles
{
    [TestFixture]
    public class FilteredFolderTests
    {
        private static FsFolder AppFsFolder => 
            (FsFolder)Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();
        
        [Test]
        public void ShouldReturnFoldersWithoutSkips()
        {
            AppFsFolder.CombineFolder("FolderCreationTest\\Subfolder\\OneMoreSubfolder").Create();
            AppFsFolder.CombineFolder("FolderCreationTest\\Subfolder2\\OneMoreSubfolder2").Create();
            AppFsFolder.CombineFolder("FolderCreationTest\\Subfolder2\\OneMoreSubfolder2\\LastOne").Create();

            var sut = AppFsFolder.CombineFolder("FolderCreationTest");

            sut = sut.Filter(true, null, new Regex("Subfolder2.*"), null, null);

            var result = sut?.Folders.Select(x => x.Value).ToList();
            
            Assert.That(result, Is.EquivalentTo(new []{"Subfolder", "Subfolder\\OneMoreSubfolder"}));
        }
        
        [Test]
        public void ShouldReturnFilesWithoutSkips()
        {
            (AppFsFolder.CombineFolder("FolderSearchingTest\\Subfolder")).Create();
            (AppFsFolder.CombineFolder("FolderSearchingTest\\Subfolder2")).Create();
            
            var sut = AppFsFolder.CombineFolder("FolderSearchingTest");
            
            Assert.That(sut, Is.Not.Null);
            
            File.WriteAllText(sut.CombineFile("test1.txt").ToString(), "A");
            File.WriteAllText(sut.CombineFile("test2.txt").ToString(), "B");
            
            File.WriteAllText(sut.CombineFile("Subfolder\\test3.txt").ToString(), "C");
            File.WriteAllText(sut.CombineFile("Subfolder\\test4.txt").ToString(), "D");
            File.WriteAllText(sut.CombineFile("Subfolder2\\test5.txt").ToString(), "E");

            sut = sut.Filter(true, null, new Regex(".*test3\\.txt"), null, null);
            
            var result = sut.Files.Select(x => x.Value).ToList();
            
            Assert.That(
                result, 
                Is.EquivalentTo(new []
                {
                    "test1.txt",
                    "test2.txt", 
                    "Subfolder\\test4.txt", 
                    "Subfolder2\\test5.txt"
                }));
            
            AppFsFolder.CombineFile("FolderSearchingTest").Remove();
        }
    }
}