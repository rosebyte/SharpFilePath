using System;
using System.IO;
using NUnit.Framework;
using SharpFilePath;
using File = System.IO.File;
using Path = SharpFilePath.Path;

namespace SharpFilepath.Tests
{
	[TestFixture]
	public class FilePathTests
	{
//		private string GetWorkingDirectory() => System.IO.Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath);
//
//		[Test]
//		public void FilePathTest()
//		{
//			const string path = "C:\\Test.txt";
//			var sut = new Path(path);
//
//			Assert.That(sut.Value, Is.EqualTo(path));
//		}
//
//		[Test]
//		public void ShouldReturnTrueOnExistingFile()
//		{
//			var sut = new Path(System.IO.Path.Combine(GetWorkingDirectory(), "TestFiles\\GetContent.txt"));
//
//			Assert.That(sut.Exists, Is.True);
//		}
//
//		[Test]
//		public void ShouldReturnFalseOnNonExistingFile()
//		{
//			var sut = new Path(System.IO.Path.Combine(GetWorkingDirectory(), "TestFiles\\NotHere.txt"));
//
//			Assert.That(sut.Exists, Is.False);
//		}
//
//		[Test]
//		public void ShouldResolvePath()
//		{
//			var sut = new Path(".\\file.txt");
//
//			Assert.That(sut.ResolvePath("C:\\SomeFolder").Value, Is.EqualTo("C:\\SomeFolder\\file.txt"));
//		}
//
//		[Test]
//		public void ShouldResolvePathWithTrailingBackSlash()
//		{
//			var sut = new Path(".\\file.txt");
//
//			Assert.That(sut.ResolvePath("C:\\SomeFolder\\").Value, Is.EqualTo("C:\\SomeFolder\\file.txt"));
//		}
//
//		[Test]
//		public void GetContentTest()
//		{
//			var myDir = GetWorkingDirectory();
//			var sut = new Path(System.IO.Path.Combine(myDir, "TestFiles\\GetContent.txt"));
//
//			Assert.That(sut.Content, Is.EqualTo("ABCD"));
//		}
//
//		[Test]
//		public void WriteTest()
//		{
//			const string working = "WORKING";
//
//			var filePath = System.IO.Path.Combine(GetWorkingDirectory(), "TestFiles\\Write.txt");
//			var sut = new Path(filePath);
//
//			File.WriteAllText(filePath, "NOT_WORKING");
//
//			sut.Write(working);
//
//			Assert.That(File.ReadAllText(filePath), Is.EqualTo(working));
//		}
//
//		[Test]
//		public void RestoreTest()
//		{
//			const string working = "WORKING";
//			const string notWorking = "NOT_WORKING";
//
//			var filePath = System.IO.Path.Combine(GetWorkingDirectory(), "TestFiles\\Restore.txt");
//			var sut = new Path(filePath);
//
//			File.WriteAllText(filePath, "NOT_WORKING");
//
//			sut.Write(working, true);
//
//			Assert.That(File.ReadAllText(filePath), Is.EqualTo(working));
//			Assert.That(sut.Backup, Is.EqualTo(notWorking));
//
//			var result = sut.Restore();
//
//			Assert.That(result, Is.True);
//			Assert.That(File.ReadAllText(filePath), Is.EqualTo(notWorking));
//		}
//
//		[Test]
//		public void ShouldNotRestoreUnchangedFile()
//		{
//			var filePath = System.IO.Path.Combine(GetWorkingDirectory(), "TestFiles\\Restore.txt");
//			var sut = new Path(filePath);
//
//			var result = sut.Restore();
//
//			Assert.That(result, Is.False);
//		}
//
//		[Test]
//		public void ToStringTest()
//		{
//			const string path = "C:\\Test.txt";
//			var sut = new Path(path);
//
//			Assert.That(sut.ToString(), Is.EqualTo(path));
//		}
	}
}