using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneAppAway._1_1.Data;
using static UnitTests.Tests;
using OneAppAway._1_1.Helpers;

namespace UnitTests
{
    [TestClass]
    public class GeneralExtensionMethodTests
    {
        [TestMethod]
        public void ToServiceDayTest()
        {
            foreach (var day in Enum.GetNames(typeof(DayOfWeek)))
            {
                DayOfWeek dayOfWeek;
                ServiceDay serviceDay;
                if (Enum.TryParse(day, out dayOfWeek) && Enum.TryParse(day, out serviceDay))
                    AssertExpectedVsActual(day, (int)serviceDay, (int)dayOfWeek.ToServiceDay());
                else
                    Assert.Fail($"{day} doesn't map to both DayOfWeek and ServiceDay enum types.");
            }
        }
    }
}
