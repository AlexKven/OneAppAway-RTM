using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneAppAway._1_1.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnitTests
{
    [TestClass]
    public class CompositeCollectionBindingTests
    {
        [TestMethod]
        public void CompositeCollectionBindingTests_AllTests()
        {
            List<int> list = new List<int>();
            CompositeCollectionBinding<int, string> binding = new CompositeCollectionBinding<int, string>(list);
            ObservableCollection<int> collection0 = new ObservableCollection<int>() { 2, 5 };
            ObservableCollection<int> collection1 = new ObservableCollection<int>();
            ObservableCollection<int> collection2 = new ObservableCollection<int>() { 3 };
            binding.AddCollection("first", collection0);
            Assert.AreEqual(list.Count, 2, "Test 0.0");
            Assert.AreEqual(list[1], 5, "Test 0.1");
            binding.AddCollection("second", collection1);
            binding.AddCollection("third", collection2);
            Assert.AreEqual(list.Count, 3, "Test 1.0");
            collection1.Add(6);
            Assert.AreEqual(list[2], 6, "Test 2.0");
            Assert.AreEqual(list[3], 3, "Test 2.1");
            collection2.Add(4);
            Assert.AreEqual(list[4], 4, "Test 3.0");
            collection0.Add(7);
            Assert.AreEqual(list[4], 3, "Test 4.0");
            Assert.AreEqual(list[5], 4, "Test 4.1");
            binding.RemoveCollection("first");
            Assert.AreEqual(list[0], 6, "Test 5.0");
            binding.RemoveCollection("second");
            binding.RemoveCollection("third");
            Assert.AreEqual(list.Count, 0, "Test 6.0");
        }
    }
}
