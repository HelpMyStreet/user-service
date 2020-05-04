using NUnit.Framework;
using UserService.Core.Extensions;

namespace UserService.UnitTests
{
    public class MathExtensionsTests
    {
        [TestCase(1, 100, 100)]
        [TestCase(99, 100, 100)]
        [TestCase(100, 100, 100)]

        [TestCase(100, 1000, 1000)]
        [TestCase(1000, 1000, 1000)]
        [TestCase(1001, 1000, 2000)]
        [TestCase(1999, 1000, 2000)]


        [TestCase(2001, 2000, 4000)]
        [TestCase(1, 2000, 2000)]
        public void RoundUpToNearest(int number, int roundTo, int expected)
        {
            var result = number.RoundUpToNearest(roundTo);
            Assert.AreEqual(expected, result);
        }
    }
}
