using System.IO;
using System.Reflection;
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
            
            
        }
        
        [OneTimeTearDown]
        public void TearDown() => _folder.Remove();
        
    }
}