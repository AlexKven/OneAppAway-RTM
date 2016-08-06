using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class RectSubsetTests
    {
        [TestMethod]
        public void RectSubsetTest1()
        {
            double width = 10;
            double height = 20;
            double leftOffset;
            double topOffset;
            RectSubset subset = new RectSubset() { Left = 2, Right = 3, Top = 8, Bottom = 5 };
            subset.Apply(ref width, ref height, out leftOffset, out topOffset);
            Assert.IsTrue(width == 5, $"width is {width}; expected 5.");
            Assert.IsTrue(height == 7, $"height is {height}; expected 7.");
            Assert.IsTrue(leftOffset == 2, $"leftOffset is {leftOffset}; expected 2.");
            Assert.IsTrue(topOffset == 8, $"leftOffset is {topOffset}; expected 8.");
        }
        [TestMethod]
        public void RectSubsetTest2()
        {
            double width = 10;
            double height = 20;
            double leftOffset;
            double topOffset;
            RectSubset subset = new RectSubset() { Left = 5, Right = 0.4, Top = 8, Bottom = 10, RightScale = RectSubsetScale.Relative, BottomValueType = RectSubsetValueType.Length };
            subset.Apply(ref width, ref height, out leftOffset, out topOffset);
            Assert.IsTrue(width == 3, $"width is {width}; expected 3.");
            Assert.IsTrue(height == 10, $"height is {height}; expected 10.");
            Assert.IsTrue(leftOffset == 5, $"leftOffset is {leftOffset}; expected 5.");
            Assert.IsTrue(topOffset == 8, $"leftOffset is {topOffset}; expected 8.");
        }
        [TestMethod]
        public void RectSubsetTest3()
        {
            double width = 10;
            double height = 20;
            double leftOffset;
            double topOffset;
            RectSubset subset = new RectSubset() { Left = 0.5, Right = 2, Top = 4, Bottom = 6, LeftScale = RectSubsetScale.Relative, LeftValueType = RectSubsetValueType.Length, TopValueType = RectSubsetValueType.Length, BottomValueType = RectSubsetValueType.Length };
            subset.Apply(ref width, ref height, out leftOffset, out topOffset);
            Assert.IsTrue(width == 4, $"width is {width}; expected 4.");
            Assert.IsTrue(height == 10, $"height is {height}; expected 10.");
            Assert.IsTrue(leftOffset == 4, $"leftOffset is {leftOffset}; expected 4.");
            Assert.IsTrue(topOffset == 5, $"leftOffset is {topOffset}; expected 5.");
        }
        [TestMethod]
        public void RectSubsetTest4()
        {
            double width = 10;
            double height = 20;
            double leftOffset;
            double topOffset;
            RectSubset subset = new RectSubset() { Left = 0.4, Right = 5, Top = 0.5, Bottom = 0.25, LeftScale = RectSubsetScale.Relative, LeftValueType = RectSubsetValueType.Length, RightValueType = RectSubsetValueType.Length, TopScale = RectSubsetScale.Relative, TopValueType = RectSubsetValueType.Length, BottomScale = RectSubsetScale.Relative, BottomValueType = RectSubsetValueType.Length };
            subset.Apply(ref width, ref height, out leftOffset, out topOffset);
            Assert.IsTrue(width == 9, $"width is {width}; expected 9.");
            Assert.IsTrue(height == 15, $"height is {height}; expected 15.");
            Assert.IsTrue(leftOffset == 0.5, $"leftOffset is {leftOffset}; expected 0.5.");
            Assert.IsTrue(topOffset == 2.5, $"leftOffset is {topOffset}; expected 2.5.");
        }
    }
}
