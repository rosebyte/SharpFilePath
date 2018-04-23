using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SharpFilePath;

namespace SharpFilepath.Tests
{
    [TestFixture]
    public class FolderTests
    {
        private static Folder AppFolder => 
            (Folder)Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();
        
        [Test]
        public void ShouldCreateFolderWithParentFolder()
        {
            ((Folder)AppFolder.Combine("FolderCreationTest\\Subfolder\\OneMoreSubfolder")).CreateIfNotExists();

            var firstFolder = System.IO.Path.Combine(AppFolder, "FolderCreationTest");
            var secondFolder = System.IO.Path.Combine(AppFolder, "FolderCreationTest\\Subfolder");
            var thirdFolder = System.IO.Path.Combine(AppFolder, "FolderCreationTest\\Subfolder\\OneMoreSubfolder");
            
            Assert.That(Directory.Exists(firstFolder), Is.True);
            Assert.That(Directory.Exists(secondFolder), Is.True);
            Assert.That(Directory.Exists(thirdFolder), Is.True);
            
            AppFolder.Combine("FolderCreationTest").Remove();
            
            Assert.That(Directory.Exists(firstFolder), Is.False);
            Assert.That(Directory.Exists(secondFolder), Is.False);
            Assert.That(Directory.Exists(thirdFolder), Is.False);
        }

        [Test]
        public void ShouldReturnFolders()
        {
            ((Folder)AppFolder.Combine("FolderCreationTest\\Subfolder\\OneMoreSubfolder")).CreateIfNotExists();
            ((Folder)AppFolder.Combine("FolderCreationTest\\Subfolder2\\OneMoreSubfolder2")).CreateIfNotExists();

            var sut = AppFolder.Combine("FolderCreationTest") as Folder;

            var result = sut?.GetFolders().Select(x => x.Value).ToList();
            
            Assert.That(result, Is.EquivalentTo(new []{"Subfolder", "Subfolder\\OneMoreSubfolder", "Subfolder2", "Subfolder2\\OneMoreSubfolder2"}));
        }
        
        [Test]
        public void ShouldReturnFiles()
        {
            ((Folder)AppFolder.Combine("FolderSearchingTest\\Subfolder")).CreateIfNotExists();
            ((Folder)AppFolder.Combine("FolderSearchingTest\\Subfolder2")).CreateIfNotExists();
            
            var sut = AppFolder.Combine("FolderSearchingTest") as Folder;
            
            Assert.That(sut, Is.Not.Null);
            
            System.IO.File.WriteAllText(sut.Combine("test1.txt"), "A");
            System.IO.File.WriteAllText(sut.Combine("test2.txt"), "B");
            
            System.IO.File.WriteAllText(sut.Combine("Subfolder\\test3.txt").ToString(), "C");
            System.IO.File.WriteAllText(sut.Combine("Subfolder\\test4.txt").ToString(), "D");
            System.IO.File.WriteAllText(sut.Combine("Subfolder2\\test5.txt").ToString(), "E");

            var result = sut?.GetFiles().Select(x => x.Value).ToList();
            
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
            
            AppFolder.Combine("FolderSearchingTest").Remove();
        }
        
        [Test]
        public void ShouldSyncStructures()
        {
            var donor = (Folder)AppFolder.Combine("FolderSyncingTestDonor");
            var acceptor = (Folder)AppFolder.Combine("FolderSyncingTestAcceptor");
            
            ((Folder)donor.Combine("Subfolder")).CreateIfNotExists();
            ((Folder)donor.Combine("Subfolder2")).CreateIfNotExists();
            System.IO.File.WriteAllText(donor.Combine("test1.txt"), "A");
            System.IO.File.WriteAllText(donor.Combine("test2.txt"), "A");
            System.IO.File.WriteAllText(donor.Combine("Subfolder\\test3.txt"), "A");
            System.IO.File.WriteAllText(donor.Combine("Subfolder2\\test4.txt"), "A");
            System.IO.File.WriteAllText(donor.Combine("Subfolder2\\test5.txt"), "A");
            
            ((Folder)acceptor.Combine("Subfolder2")).CreateIfNotExists();
            ((Folder)acceptor.Combine("Subfolder3")).CreateIfNotExists();
            System.IO.File.WriteAllText(acceptor.Combine("test1.txt"), "A");
            System.IO.File.WriteAllText(acceptor.Combine("test3.txt"), "A");
            System.IO.File.WriteAllText(acceptor.Combine("Subfolder2\\test4.txt"), "A");
            System.IO.File.WriteAllText(acceptor.Combine("Subfolder2\\test1.txt"), "A");
            System.IO.File.WriteAllText(acceptor.Combine("Subfolder3\\test5.txt"), "A");
            
            donor.SyncStructure(acceptor);

            var files = acceptor.GetFiles().Select(x => x.Value);
            var folders = acceptor.GetFolders().Select(x => x.Value);
            
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

            AppFolder.Combine("FolderSyncingTestDonor").Remove();
            AppFolder.Combine("FolderSyncingTestAcceptor").Remove();
        }
    }
}