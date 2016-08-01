using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Dsp.Tests
{
    [TestClass]
    public sealed class OnlineFilterExtensionsTests
    {
        [TestMethod]
        public void NullFilterDoesNothing()
        {
            Assert.AreEqual(100, NullFilter.Instance.Process(100));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsForNullInput1()
        {
            IOnlineFilter filter = null;
            filter.Process(new double[] { 1, 2, 3 });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsForNullInput2()
        {
            NullFilter.Instance.Process(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsForInvalidInput1()
        {
            NullFilter.Instance.Process(new double[] { 1, 2, 3 }, -1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsForInvalidInput2()
        {
            NullFilter.Instance.Process(new double[] { 1, 2, 3 }, 3, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsForInvalidInput3()
        {
            NullFilter.Instance.Process(new double[] { 1, 2, 3 }, 0, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ThrowsForInvalidInput4()
        {
            NullFilter.Instance.Process(new double[] { 1, 2, 3 }, 0, 4);
        }

        [TestMethod]
        public void CreatesNewArrayForOutput()
        {
            double[] input = { 1, 2, 3 };
            double[] output = NullFilter.Instance.Process(input);

            Assert.IsFalse(Object.ReferenceEquals(input, output));
            AssertAreEqual(input, output);
        }

        [TestMethod]
        public void FiltersSubsetOfInput()
        {
            double[] input = { 1, 2, 3 };
            using (var filter = new ZeroFilter())
            {
                double[] output = filter.Process(input, 1, 1);
                Assert.AreEqual(1, output.Length, "Output array is sized according to number of samples to filter (regardless input array size).");
                Assert.AreEqual(0, output[0]);

                filter.ProcessInPlace(input, 1, 1);
                Assert.AreEqual(0, input[1]);
            }
        }

        [TestMethod]
        public void FiltersInPlace()
        {
            double[] input = { 1, 2, 3 };
            using (var filter = new ZeroFilter())
            {
                filter.ProcessInPlace(input);

                Assert.AreEqual(0, input[0]);
                Assert.AreEqual(0, input[1]);
                Assert.AreEqual(0, input[2]);
            }            
        }

        private sealed class ZeroFilter : IOnlineFilter
        {
            public double Process(double sample)
            {
                return 0;
            }

            public void Reset()
            {
            }

            public void Dispose()
            {
            }
        }

        private static void AssertAreEqual(double[] expectedValues, double[] values)
        {
            Assert.IsTrue((expectedValues == null) == (values == null));
            if (expectedValues == null)
                return;

            Assert.AreEqual(expectedValues.Length, values.Length, "Arrays must have same length");
            for (int i = 0; i < expectedValues.Length; ++i)
                Assert.AreEqual(expectedValues[i], values[i]);
        }

    }
}
