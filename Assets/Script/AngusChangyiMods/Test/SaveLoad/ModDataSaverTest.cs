using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Script.AngusChangyiMods.Core.Util;

namespace AngusChangyiMods.Core.SaveLoad.Test
{
    [TestFixture]
    public class FileModDataSaverTests
    {
        public class SampleSettings
        {
            public string Name { get; set; }
            public int Volume { get; set; }
        }

        public class NestedSettings
        {
            public string Name { get; set; }
            public SampleSettings Inner { get; set; }
        }

        public class NoParameterlessConstructor
        {
            public string Value;

            public NoParameterlessConstructor(string value)
            {
                Value = value;
            }
        }

        private class PrivateType
        {
            public string Name { get; set; }

            public PrivateType()
            {
            }
        }

        public class FieldWithUnsupportedType
        {
            public System.IO.Stream stream; // not serializable
        }

        private string tempDir;
        private FileModDataSaver saver;


        [SetUp]
        public void SetUp()
        {
            tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            saver = new FileModDataSaver(tempDir);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Test]
        public void Write_ShouldCreateFile_WithCorrectJson()
        {
            var data = new SampleSettings { Name = "TestName", Volume = 42 };

            saver.Write(data);

            string filePath = Path.Combine(tempDir, TypeFileNameConverter.GetFileNameForType(typeof(SampleSettings), ModDataSaver.FileExtension));
            Assert.That(File.Exists(filePath));
    
            string content = File.ReadAllText(filePath);
            Assert.That(content, Does.Contain("TestName"));
            Assert.That(content, Does.Contain("42"));
        }


        [Test]
        public void Write_ListOfItems_ShouldCreateFile()
        {
            var data = new List<SampleSettings>
            {
                new SampleSettings { Name = "A", Volume = 1 },
                new SampleSettings { Name = "B", Volume = 2 }
            };

            saver.Write(data);

            string expectedFile = TypeFileNameConverter.GetFileNameForType(typeof(List<SampleSettings>), ModDataSaver.FileExtension);
            string filePath = Path.Combine(tempDir, expectedFile);

            Console.WriteLine("Looking for: " + expectedFile);
            foreach (var f in Directory.GetFiles(tempDir))
            {
                Console.WriteLine("Found file: " + Path.GetFileName(f));
            }

            Assert.That(File.Exists(filePath), Is.True, $"File should exist: {filePath}");
        }


        [Test]
        public void Write_NestedObject_ShouldSerializeCorrectly()
        {
            var data = new NestedSettings
            {
                Name = "Parent",
                Inner = new SampleSettings { Name = "Child", Volume = 7 }
            };

            saver.Write(data);

            string filePath = Path.Combine(tempDir, TypeFileNameConverter.GetFileNameForType(typeof(NestedSettings), ModDataSaver.FileExtension));
            Assert.That(File.Exists(filePath), Is.True);

            string content = File.ReadAllText(filePath);
            Assert.That(content, Does.Contain("Parent"));
            Assert.That(content, Does.Contain("Child"));
        }


        [Test]
        public void FileName_ShouldMatch_TypeFullName()
        {
            saver.Write(new SampleSettings());

            string expectedFile = TypeFileNameConverter.GetFileNameForType(typeof(SampleSettings), ModDataSaver.FileExtension);
            string[] files = Directory.GetFiles(tempDir);
            string actualFile = Path.GetFileName(files[0]);

            Assert.That(actualFile, Is.EqualTo(expectedFile));
        }


        [Test]
        public void Write_ListStringAndListInt_ShouldNotShareFile()
        {
            saver.Write(new List<string> { "hi" });
            saver.Write(new List<int> { 42 });

            string file1 = Path.Combine(tempDir, TypeFileNameConverter.GetFileNameForType(typeof(List<string>), ModDataSaver.FileExtension));
            string file2 = Path.Combine(tempDir, TypeFileNameConverter.GetFileNameForType(typeof(List<int>), ModDataSaver.FileExtension));

            Assert.That(File.Exists(file1), Is.True, $"Expected file1 exists: {file1}");
            Assert.That(File.Exists(file2), Is.True, $"Expected file2 exists: {file2}");
        }

        [Test]
        public void Write_ShouldThrow_WhenNoParameterlessConstructor()
        {
            var obj = new NoParameterlessConstructor("abc");

            var ex = Assert.Throws<InvalidOperationException>(() => { saver.Write(obj); });

            Assert.That(ex.Message, Does.Contain("must have a public parameterless constructor"));
        }

        [Test]
        public void Write_ShouldThrow_WhenTypeIsPrivate()
        {
            var obj = new PrivateType();

            var ex = Assert.Throws<InvalidOperationException>(() => { saver.Write(obj); });

            Assert.That(ex.Message, Does.Contain("must be public"));
        }

        [Test]
        public void Write_ShouldThrow_WhenFieldIsNotSerializable()
        {
            var obj = new FieldWithUnsupportedType
            {
                stream = Console.OpenStandardInput()
            };

            var ex = Assert.Throws<InvalidOperationException>(() => { saver.Write(obj); });

            Assert.That(ex.Message, Does.Contain("which may not be serializable"));
        }
    }
}