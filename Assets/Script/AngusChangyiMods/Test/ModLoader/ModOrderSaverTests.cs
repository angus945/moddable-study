using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using AngusChangyiMods.Core.Test;

namespace AngusChangyiMods.Core.SaveLoad.Test
{
    [TestFixture]
    public class ModOrderSaverTests
    {
        private class FailingSaver : IModDataSaver
        {
            public bool TryRead<T>(out T data) where T : class
            {
                data = null;
                throw new System.Exception("Simulated load failure");
            }

            public void Write<T>(T data) where T : class
            {
                throw new System.Exception("Simulated save failure");
            }

            public T Read<T>() where T : class => throw new System.NotImplementedException();
            public bool Has<T>() where T : class => false;
            public void Delete<T>() where T : class => throw new System.NotImplementedException();
        }
        
        private string tempDir;
        private FileModDataSaver fileSaver;
        private ModOrderSaver modOrderSaver;

        [SetUp]
        public void SetUp()
        {
            tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            fileSaver = new FileModDataSaver(tempDir);
            modOrderSaver = new ModOrderSaver(fileSaver);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }

        private ModSortingData CreateSortingData(string id, string dir)
        {
            return new ModSortingData(id, dir);
        }

        [Test]
        public void SaveModOrder_ShouldWriteToFile()
        {
            var sortList = new List<ModSortingData>
            {
                CreateSortingData("com.test.mod1", "dir1"),
                CreateSortingData("com.test.mod2", "dir2")
            };

            modOrderSaver.SaveModOrder(sortList);

            // 確認檔案已存在
            Assert.That(fileSaver.Has<ModOrderSaver.ModOrder>(), Is.True);

            // 再讀出來驗證
            var readBack = modOrderSaver.LoadModOrder();
            Assert.That(readBack.Count, Is.EqualTo(2));
            Assert.That(readBack[0].packageId, Is.EqualTo("com.test.mod1"));
        }

        [Test]
        public void LoadModOrder_ShouldReturnEmptyList_WhenFileMissing()
        {
            var result = modOrderSaver.LoadModOrder();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void SaveModOrder_ShouldNotThrow_WhenSaverFails()
        {
            var logger = new MockLogger();
            var saver = new FailingSaver();
            var modOrderSaver = new ModOrderSaver(saver, logger);

            Assert.DoesNotThrow(() => modOrderSaver.SaveModOrder(new List<ModSortingData>()));
            Assert.That(logger.Logs.Count, Is.GreaterThan(0));
            Assert.That(logger.Logs[0].Message, Does.StartWith(ModOrderSaver.errorSavingModOrder));
        }

        [Test]
        public void LoadModOrder_ShouldReturnEmptyList_WhenSaverThrows()
        {
            var logger = new MockLogger();
            var saver = new FailingSaver();
            var modOrderSaver = new ModOrderSaver(saver, logger);

            var result = modOrderSaver.LoadModOrder();
            Assert.That(result, Is.Empty);
            Assert.That(logger.Logs.Count, Is.GreaterThan(0));
            Assert.That(logger.Logs[0].Message, Does.StartWith(ModOrderSaver.errorLoadingModOrder));
        }
    }
}