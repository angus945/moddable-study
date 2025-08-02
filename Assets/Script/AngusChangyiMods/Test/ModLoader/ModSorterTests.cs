using System.Collections.Generic;
using NUnit.Framework;
using AngusChangyiMods.Core;

namespace AngusChangyiMods.Core.ModLoader.Tests
{
    [TestFixture]
    public class ModSorterTests
    {
        private ModSorter sorter;

        [SetUp]
        public void SetUp()
        {
            sorter = new ModSorter();
        }

        private ModContentPack CreatePack(string id, string dir)
        {
            return new ModContentPack(new ModMetaData("Test", id, "Author", "Desc", dir));
        }

        private ModSortingData CreateSort(string id, string dir)
        {
            return new ModSortingData(id, dir);
        }

        [Test]
        public void LoadOrder_ShouldMatchModContentPacksToSorts()
        {
            var mod1 = CreatePack("mod.a", "folder1");
            var mod2 = CreatePack("mod.b", "folder2");

            var sortList = new List<ModSortingData>
            {
                CreateSort("mod.a", "folder1")
            };

            sorter.LoadOrder(new List<ModContentPack> { mod1, mod2 }, sortList);

            Assert.That(sorter.SortedMods.Count, Is.EqualTo(2));
            Assert.That(sorter.SortedMods[0].relatedContentPack, Is.EqualTo(mod1));
            Assert.That(sorter.SortedMods[1].relatedContentPack, Is.EqualTo(mod2));
        }

        [Test]
        public void LoadOrder_ShouldCreateMissingSorts()
        {
            var mod1 = CreatePack("mod.x", "somewhere");

            sorter.LoadOrder(new List<ModContentPack> { mod1 }, new List<ModSortingData>());

            Assert.That(sorter.SortedMods.Count, Is.EqualTo(1));
            Assert.That(sorter.SortedMods[0].packageId, Is.EqualTo("mod.x"));
            Assert.That(sorter.SortedMods[0].relatedContentPack, Is.EqualTo(mod1));
        }

        [Test]
        public void SetOrder_ShouldReplaceSortedMods()
        {
            var customSort = new List<ModSortingData>
            {
                CreateSort("mod.1", "dir/1"),
                CreateSort("mod.2", "dir/2")
            };

            sorter.SetOrder(customSort);

            Assert.That(sorter.SortedMods, Is.EqualTo(customSort));
        }
    }
}
