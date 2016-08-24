using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class OneAppAwayTests
    {
        [TestMethod]
        public void UIDataSourceTest()
        {
            int counter = 0;
            TestDataSource test = new TestDataSource();
            EventHandler dataChanged = delegate(object sender, EventArgs e)
            {
                counter += test.Data;
            };
            EventHandler decommissioned = null;
            decommissioned = delegate (object sender, EventArgs e)
            {
                test.DataChanged -= dataChanged;
                test.Decommissioned -= decommissioned;
            };
            test.DataChanged += dataChanged;
            test.Decommissioned += decommissioned;
            test.Data = 1;
            test.Data = 2;
            test.Data = 3;
            test.Decommission();
            test.Data = 4;
            Assert.IsTrue(counter == 6, "Counter value is " + counter.ToString());
        }
    }
}
