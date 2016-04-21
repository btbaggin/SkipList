using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SkipList
{
    [TestClass]
    public class SkipListTest
    {
        SkipList<int> s;
        public SkipListTest()
        {
            s = new SkipList<int>();
            s.Add(10);
            s.Add(5);
            s.Add(1);
            s.Add(15);
        }

        [TestMethod]
        public void CountTest()
        {
            Assert.AreEqual(s.Count, 4);
        }

        [TestMethod]
        public void ContainsTest()
        {
            Assert.IsTrue(s.Contains(1));
            Assert.IsTrue(s.Contains(5));
            Assert.IsTrue(s.Contains(10));
            Assert.IsTrue(s.Contains(15));
        }

        [TestMethod]
        public void IndexTest()
        {
            Assert.AreEqual(s.Index(0), 1);
            Assert.AreEqual(s.Index(1), 5);
            Assert.AreEqual(s.Index(2), 10);
            Assert.AreEqual(s.Index(3), 15);
        }

        [TestMethod]
        public void AddRemoveTest()
        {
            s.Add(11);
            Assert.IsTrue(s.Contains(11));
            s.Remove(11);
            Assert.IsFalse(s.Contains(11));
        }

        [TestMethod]
        public void ClearTest()
        {
            s.Clear();
            Assert.IsFalse(s.Contains(1));
            Assert.IsFalse(s.Contains(5));
            Assert.IsFalse(s.Contains(10));
            Assert.IsFalse(s.Contains(15));
            Assert.AreEqual(s.Count, 0);

            s.Add(10);
            s.Add(5);
            s.Add(1);
            s.Add(15);
        }

        [TestMethod]
        public void OrderedTest()
        {
            for (int i = 0; i < s.Count - 1; i++)
            {
                Assert.IsTrue(s.Index(i) < s.Index(i + 1));
            }
        }
    }
}
