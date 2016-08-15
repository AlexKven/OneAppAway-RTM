using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneAppAway._1_1.Data;

namespace UnitTests
{
    [TestClass]
    public class SqlProviderExtensionsTests
    {
        private void GenericTryToNumberTest<T>(T testCase) where T : struct, IEquatable<T>
        {
            string[,] sqlr = new string[1, 1];
            sqlr[0, 0] = testCase.ToString();
            T result;
            bool success;
            if (!(success = sqlr.TryToNumber<T>(out result)) || !result.Equals(testCase))
            {
                Assert.Fail(success ? $"({typeof(T).FullName}){testCase.ToString()} parsed to {result.ToString()}." : $"Type {typeof(T).FullName} was not able to be parsed.");
            }
        }

        [TestMethod]
        public void TryToNumberTests()
        {
            GenericTryToNumberTest((int)5);
            GenericTryToNumberTest((long)5);
            GenericTryToNumberTest((byte)5);
            GenericTryToNumberTest((short)5);
            GenericTryToNumberTest((uint)5);
            GenericTryToNumberTest((ulong)5);
            GenericTryToNumberTest((sbyte)5);
            GenericTryToNumberTest((ushort)5);
            GenericTryToNumberTest((double)5.32);
            GenericTryToNumberTest((float)5.32);
            GenericTryToNumberTest(true);
            GenericTryToNumberTest((decimal)5.32);
        }
    }
}
