using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using RoseByte.SharpFiles.Extensions;
using RoseByte.SharpFiles.Internal;
using File = System.IO.File;

namespace RoseByte.SharpFiles.Tests
{
    [TestFixture]
    public class FolderTests
    {
        private static FsFolder AppFsFolder => 
            (FsFolder)Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();

        [OneTimeSetUp]
        public void CreateTestingStructure()
        {
            AppFsFolder.CombineFolder(nameof(ShouldCreateFolderWithParentFolder)).Create();

            var firstFolder = Path.Combine(AppFsFolder, "FolderCreationTest");
            var secondFolder = Path.Combine(AppFsFolder, "FolderCreationTest\\Subfolder");
            var thirdFolder =Path.Combine(AppFsFolder, "FolderCreationTest\\Subfolder\\OneMoreSubfolder");
        }
        
        [OneTimeTearDown]
        public void RemoveTestingStructure()
        {
            
        }
        
        [Test]
        public void ShouldCreateFolderWithParentFolder()
        {
            AppFsFolder.CombineFolder("FolderCreationTest\\Subfolder\\OneMoreSubfolder").Create();

            var firstFolder = Path.Combine(AppFsFolder, "FolderCreationTest");
            var secondFolder = Path.Combine(AppFsFolder, "FolderCreationTest\\Subfolder");
            var thirdFolder =Path.Combine(AppFsFolder, "FolderCreationTest\\Subfolder\\OneMoreSubfolder");
            
            Assert.That(Directory.Exists(firstFolder), Is.True);
            Assert.That(Directory.Exists(secondFolder), Is.True);
            Assert.That(Directory.Exists(thirdFolder), Is.True);
            
            AppFsFolder.CombineFile("FolderCreationTest").Remove();
            
            Assert.That(Directory.Exists(firstFolder), Is.False);
            Assert.That(Directory.Exists(secondFolder), Is.False);
            Assert.That(Directory.Exists(thirdFolder), Is.False);
        }
        
        [Test]
        public void ShouldCreateFolderInstance()
        {
            var path = "C:\\";
            var sut = path.ToFolder();
            Assert.That(sut.Value, Is.EqualTo(path));
        }
        
        [Test]
        public void ShouldReturnFalseToIsFolder()
        {
            var sut = "C:\\".ToFolder();
            Assert.That(sut.IsFile, Is.False);
            Assert.That(sut.IsFolder, Is.True);
        }
        
        [Test]
        public void ShouldCombineFile()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = Path.Combine(dir).ToPath();
            var parent = sut.Parent;
            var name = sut.ToString().Split('\\').Last();
            
            Assert.That(parent.CombineFolder(name).ToString(), Is.EqualTo(sut.ToString()));
        }
        
        [Test]
        public void ShouldCombineFolder()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = Path.Combine(dir).ToPath();
            var parent = sut.Parent;
            var name = sut.ToString().Split('\\').Last();
            
            Assert.That(parent.CombineFolder(name).ToString(), Is.EqualTo(sut.ToString()));
        }
        
        [Test]
        public void ShouldGetParentDirectory()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var parentDir = Directory.GetParent(dir).FullName;
            var sut = Path.Combine(dir).ToPath();
            var parent = sut.Parent;
            
            Assert.That(parent.ToString(), Is.EqualTo(parentDir));
        }
        
        [Test]
        public void ShouldCreateFolderWithDot()
        {
            var sut = new Folder("C:\\Test.Folder");
            
            Assert.That(sut.ToString(), Is.EqualTo("C:\\Test.Folder"));
        }

        [Test]
        public void ShouldReturnFolders()
        {
            AppFsFolder.CombineFolder("FolderCreationTest\\Subfolder\\OneMoreSubfolder").Create();
            AppFsFolder.CombineFolder("FolderCreationTest\\Subfolder2\\OneMoreSubfolder2").Create();

            var sut = AppFsFolder.CombineFolder("FolderCreationTest");

            var result = sut?.Folders.Select(x => x.Value).ToList();
            
            Assert.That(result, Is.EquivalentTo(new []{"Subfolder", "Subfolder\\OneMoreSubfolder", "Subfolder2", "Subfolder2\\OneMoreSubfolder2"}));
        }
        
        [Test]
        public void ShouldReturnFiles()
        {
            AppFsFolder.CombineFolder("FolderSearchingTest\\Subfolder").Create();
            AppFsFolder.CombineFolder("FolderSearchingTest\\Subfolder2").Create();
            
            var sut = AppFsFolder.CombineFolder("FolderSearchingTest");
            
            Assert.That(sut, Is.Not.Null);
            
            File.WriteAllText(sut.CombineFolder("test1.txt").ToString(), "A");
            File.WriteAllText(sut.CombineFolder("test2.txt").ToString(), "B");
            
            File.WriteAllText(sut.CombineFolder("Subfolder\\test3.txt").ToString(), "C");
            File.WriteAllText(sut.CombineFolder("Subfolder\\test4.txt").ToString(), "D");
            File.WriteAllText(sut.CombineFolder("Subfolder2\\test5.txt").ToString(), "E");

            var result = sut.Files.Select(x => x.Value).ToList();
            
            Assert.That(
                result, 
                Is.EquivalentTo(new []
                {
                    "test1.txt",
                    "test2.txt", 
                    "Subfolder\\test3.txt", 
                    "Subfolder\\test4.txt", 
                    "Subfolder2\\test5.txt"
                }));
            
            AppFsFolder.CombineFile("FolderSearchingTest").Remove();
        }
        
        [Test]
        public void ShouldSyncStructures()
        {
            var donor = AppFsFolder.CombineFolder("FolderSyncingTestDonor");
            var acceptor = AppFsFolder.CombineFolder("FolderSyncingTestAcceptor");
            
            donor.CombineFolder("Subfolder").Create();
            donor.CombineFolder("Subfolder2").Create();
            File.WriteAllText(donor.CombineFile("test1.txt").ToString(), "A");
            File.WriteAllText(donor.CombineFile("test2.txt").ToString(), "A");
            File.WriteAllText(donor.CombineFile("Subfolder\\test3.txt").ToString(), "A");
            File.WriteAllText(donor.CombineFile("Subfolder2\\test4.txt").ToString(), "A");
            File.WriteAllText(donor.CombineFile("Subfolder2\\test5.txt").ToString(), "A");
            
            acceptor.CombineFolder("Subfolder2").Create();
            acceptor.CombineFolder("Subfolder3").Create();
            File.WriteAllText(acceptor.CombineFile("test1.txt").ToString(), "A");
            File.WriteAllText(acceptor.CombineFile("test3.txt").ToString(), "A");
            File.WriteAllText(acceptor.CombineFile("Subfolder2\\test4.txt").ToString(), "A");
            File.WriteAllText(acceptor.CombineFile("Subfolder2\\test1.txt").ToString(), "A");
            File.WriteAllText(acceptor.CombineFile("Subfolder3\\test5.txt").ToString(), "A");
            
            donor.SyncStructure(acceptor);

            var files = acceptor.Files.Select(x => x.Value);
            var folders = acceptor.Folders.Select(x => x.Value);
            
            Assert.That(
                folders, 
                Is.EquivalentTo(new []
                {
                    "Subfolder",
                    "Subfolder2"
                }));
            
            Assert.That(
                files, 
                Is.EquivalentTo(new []
                {
                    "test1.txt",
                    "Subfolder2\\test4.txt"
                }));

            AppFsFolder.CombineFile("FolderSyncingTestDonor").Remove();
            AppFsFolder.CombineFile("FolderSyncingTestAcceptor").Remove();
        }
        
        [Test]
        public void ShouldSyncStructuresWithoutSkips()
        {
            var donor = AppFsFolder.CombineFolder("FolderSyncingTestDonor");
            var acceptor = AppFsFolder.CombineFolder("FolderSyncingTestAcceptor");
            
            (donor.CombineFolder("Subfolder")).Create();
            (donor.CombineFolder("Subfolder2")).Create();
            File.WriteAllText(donor.CombineFile("test1.txt").ToString(), "A");
            File.WriteAllText(donor.CombineFile("test2.txt").ToString(), "A");
            File.WriteAllText(donor.CombineFile("Subfolder\\test3.txt").ToString(), "A");
            File.WriteAllText(donor.CombineFile("Subfolder2\\test4.txt").ToString(), "A");
            File.WriteAllText(donor.CombineFile("Subfolder2\\test5.txt").ToString(), "A");
            
            (acceptor.CombineFolder("Subfolder2")).Create();
            (acceptor.CombineFolder("Subfolder3")).Create();
            File.WriteAllText(acceptor.CombineFile("test1.txt").ToString(), "A");
            File.WriteAllText(acceptor.CombineFile("test3.txt").ToString(), "A");
            File.WriteAllText(acceptor.CombineFile("Subfolder2\\test4.txt").ToString(), "A");
            File.WriteAllText(acceptor.CombineFile("Subfolder2\\test1.txt").ToString(), "A");
            File.WriteAllText(acceptor.CombineFile("Subfolder3\\test5.txt").ToString(), "A");

            donor = donor.Filter(true, new Regex(".*Subfolder3.*"), new Regex(".*test1\\.txt"));
            
            var files = acceptor.Files.Select(x => x.Value);
            var folders = acceptor.Folders.Select(x => x.Value);
            
            Assert.That(folders, Is.EquivalentTo(new []
            {
                "Subfolder",
                "Subfolder2",
                "Subfolder3"
            }));
            
            Assert.That(
                files, 
                Is.EquivalentTo(new []
                {
                    "test1.txt",
                    "Subfolder2\\test4.txt",
                    "Subfolder2\\test1.txt",
                    "Subfolder3\\test5.txt"
                }));

            AppFsFolder.CombineFile("FolderSyncingTestDonor").Remove();
            AppFsFolder.CombineFile("FolderSyncingTestAcceptor").Remove();
        }
        
        [Test]
        public void ShouldSkipFileStartingWithAsterisk()
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

            sut = sut.Filter(true, null, new Regex(".*test3\\.txt"));
            
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