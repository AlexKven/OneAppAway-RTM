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
    public class Tests
    {
        [TestMethod]
        public void RectSubset_Test1()
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
        public void RectSubset_Test2()
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
        public void RectSubset_Test3()
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
        public void RectSubset_Test4()
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

        internal static void AssertExpectedVsActual<T>(string valueName, T expected, T actual) where T : IEquatable<T>
        {
            if (!expected.Equals(actual))
                Assert.Fail($"{valueName} value is incorrect. Expected = {expected.ToString()}. Actual = {actual.ToString()}.");
        }

        internal static void AssertExpectedException<E>(string operationDescription, Action operation) where E : Exception
        {
            bool exceptionThrown = false;
            try
            {
                operation();
            }
            catch (E)
            {
                exceptionThrown = true;
            }
            finally
            {
                if (!exceptionThrown)
                    Assert.Fail($"{operationDescription} didn't throw the expected {typeof(E).Name}.");
            }
        }

        [TestMethod]
        public void LatLonRect_InstantiationTests()
        {
            LatLonRect rect = new LatLonRect(2, 3, 4, 5);
            LatLonRect expected = new LatLonRect(new LatLon(2, 3), new LatLon(4, 5));
            AssertExpectedVsActual("rect1", expected, rect);
            rect = LatLonRect.FromPointAndSpan(new LatLon(4, 6), new LatLon(7, 8));
            expected = new LatLonRect(11, 14, 4, 6);
            AssertExpectedVsActual("rect2", expected, rect);
            rect = LatLonRect.FromNWSE(new LatLon(5, 6), new LatLon(7, 8));
            expected = new LatLonRect(5, 8, 7, 6);
            AssertExpectedVsActual("rect3", expected, rect);
            rect = LatLonRect.Parse("5, 6, 7, 8");
            expected = new LatLonRect(5, 6, 7, 8);
            AssertExpectedVsActual("rect4", expected, rect);
        }

        [TestMethod]
        public void LatLonRect_RectSubsetTests()
        {
            //LatLonRect rect = LatLonRect.FromNWSE(new LatLon(12, 4), new LatLon(2, 24));
            LatLonRect rect = LatLonRect.FromPointAndSpan(2, 4, 10, 20);
            LatLonRect result = rect.ApplySubset(new RectSubset() { Left = 1, Right = 2, Top = 3, Bottom = 4 });
            LatLonRect expected = LatLonRect.FromPointAndSpan(6, 5, 3, 17);
            AssertExpectedVsActual("rect1", expected, result);
            result = rect.ApplySubset(new RectSubset() { Left = 0.25, LeftScale = RectSubsetScale.Relative, Right = 0.5, RightScale = RectSubsetScale.Relative, Top = 0.1, TopScale = RectSubsetScale.Relative, Bottom = 0.2, BottomScale = RectSubsetScale.Relative });
            expected = LatLonRect.FromPointAndSpan(4, 9, 7, 5);
            AssertExpectedVsActual("rect2", expected, result);
            result = rect.ApplySubset(new RectSubset() { Left = -0.25, LeftScale = RectSubsetScale.Relative, Right = 10, Top = 0.1, TopScale = RectSubsetScale.Relative, Bottom = 0.2, BottomScale = RectSubsetScale.Relative, BottomValueType = RectSubsetValueType.Length });
            expected = LatLonRect.FromPointAndSpan(9, 1.5, 2, 12.5);
            AssertExpectedVsActual("rect3", expected, result);
            result = rect.ApplySubset(new RectSubset() { Left = 10, LeftValueType = RectSubsetValueType.Length, Top = 5, TopValueType = RectSubsetValueType.Length, RightValueType = RectSubsetValueType.Length, BottomValueType = RectSubsetValueType.Length });
            expected = LatLonRect.FromPointAndSpan(4.5, 9, 5, 10);
            AssertExpectedVsActual("rect4", expected, result);
        }

        [TestMethod]
        public void LatLonRect_Miniaturize()
        {
            LatLonRect rect = LatLonRect.FromPointAndSpan(2, 4, -10, 18);
            var mini = rect.Miniaturize(new LatLon(5, 7));
            var miniEnumerator = mini.GetEnumerator();
            AssertExpectedException<InvalidOperationException>("Forgetting to MoveNext after creating enumerator", () => miniEnumerator.Current.ToString());
            AssertExpectedVsActual("moveNext0", true, miniEnumerator.MoveNext());
            AssertExpectedVsActual("enumeration0", LatLonRect.FromPointAndSpan(2, 4, -5, 6), miniEnumerator.Current);
            AssertExpectedVsActual("moveNext1", true, miniEnumerator.MoveNext());
            AssertExpectedVsActual("enumeration1", LatLonRect.FromPointAndSpan(-3, 4, -5, 6), miniEnumerator.Current);
            AssertExpectedVsActual("moveNext2", true, miniEnumerator.MoveNext());
            AssertExpectedVsActual("enumeration2", LatLonRect.FromPointAndSpan(2, 10, -5, 6), miniEnumerator.Current);
            AssertExpectedVsActual("moveNext3", true, miniEnumerator.MoveNext());
            AssertExpectedVsActual("enumeration3", LatLonRect.FromPointAndSpan(-3, 10, -5, 6), miniEnumerator.Current);
            AssertExpectedVsActual("moveNext4", true, miniEnumerator.MoveNext());
            AssertExpectedVsActual("enumeration4", LatLonRect.FromPointAndSpan(2, 16, -5, 6), miniEnumerator.Current);
            AssertExpectedVsActual("moveNext5", true, miniEnumerator.MoveNext());
            AssertExpectedVsActual("enumeration5", LatLonRect.FromPointAndSpan(-3, 16, -5, 6), miniEnumerator.Current);
            AssertExpectedVsActual("moveNext6", false, miniEnumerator.MoveNext());
            AssertExpectedException<InvalidOperationException>("Moving past end of enumerator", () => miniEnumerator.Current.ToString());
            miniEnumerator.Reset();
            AssertExpectedVsActual("moveNext0b", true, miniEnumerator.MoveNext());
            AssertExpectedVsActual("enumeration0b", LatLonRect.FromPointAndSpan(2, 4, -5, 6), miniEnumerator.Current);
        }

        [TestMethod]
        public void LatLonRect_Intersects()
        {
            LatLonRect rect = new LatLonRect(8, 4, 2, -8);
            LatLonRect other = new LatLonRect(4, 10, 0, 0);
            AssertExpectedVsActual("intersects0", true, rect.Intersects(other));
            other = new LatLonRect(4, 10, 0, 6);
            AssertExpectedVsActual("intersects1", false, rect.Intersects(other));
            other = new LatLonRect(1, 10, 0, 0);
            AssertExpectedVsActual("intersects2", false, rect.Intersects(other));
            other = new LatLonRect(12, 10, 10, 0);
            AssertExpectedVsActual("intersects3", false, rect.Intersects(other));
            other = new LatLonRect(4, -20, 0, -10);
            AssertExpectedVsActual("intersects4", false, rect.Intersects(other));
        }

        [TestMethod]
        public void LatLonRect_GetNewRegionTests()
        {
            LatLonRect rect = new LatLonRect(8, 10, 4, 5);
            var reg = rect.GetNewRegion(new LatLonRect(7, 12, 4, 4)).ToList();
            var expected = new LatLonRect(7, 12, 4, 10);
            if (!reg.Contains(expected))
                Assert.Fail($"GetNewRegion0: expected to contain {expected}, which it does not.");
            reg.Remove(expected);
            expected = new LatLonRect(7, 5, 4, 4);
            if (!reg.Contains(expected))
                Assert.Fail($"GetNewRegion0: expected to contain {expected}, which it does not.");
            reg.Remove(expected);
            if (reg.Count > 0)
                Assert.Fail($"GetNewRegion0: result contains unexpected entries:{reg.Aggregate("", (acc, rct) => acc + $" ({rct.ToString()})")}.");

            reg = rect.GetNewRegion(new LatLonRect(9, 12, 1, 1)).ToList();
            expected = new LatLonRect(9, 10, 8, 5);
            if (!reg.Contains(expected))
                Assert.Fail($"GetNewRegion1: expected to contain {expected}, which it does not.");
            reg.Remove(expected);
            expected = new LatLonRect(9, 12, 8, 10);
            if (!reg.Contains(expected))
                Assert.Fail($"GetNewRegion1: expected to contain {expected}, which it does not.");
            reg.Remove(expected);
            expected = new LatLonRect(8, 12, 4, 10);
            if (!reg.Contains(expected))
                Assert.Fail($"GetNewRegion1: expected to contain {expected}, which it does not.");
            reg.Remove(expected);
            expected = new LatLonRect(4, 12, 1, 10);
            if (!reg.Contains(expected))
                Assert.Fail($"GetNewRegion1: expected to contain {expected}, which it does not.");
            reg.Remove(expected);
            expected = new LatLonRect(4, 10, 1, 5);
            if (!reg.Contains(expected))
                Assert.Fail($"GetNewRegion1: expected to contain {expected}, which it does not.");
            reg.Remove(expected);
            expected = new LatLonRect(4, 5, 1, 1);
            if (!reg.Contains(expected))
                Assert.Fail($"GetNewRegion1: expected to contain {expected}, which it does not.");
            reg.Remove(expected);
            expected = new LatLonRect(8, 5, 4, 1);
            if (!reg.Contains(expected))
                Assert.Fail($"GetNewRegion1: expected to contain {expected}, which it does not.");
            reg.Remove(expected);
            expected = new LatLonRect(9, 5, 8, 1);
            if (!reg.Contains(expected))
                Assert.Fail($"GetNewRegion1: expected to contain {expected}, which it does not.");
            reg.Remove(expected);
            if (reg.Count > 0)
                Assert.Fail($"GetNewRegion1: result contains unexpected entries:{reg.Aggregate("", (acc, rct) => acc + $" ({rct.ToString()})")}.");
        }
    }
}
