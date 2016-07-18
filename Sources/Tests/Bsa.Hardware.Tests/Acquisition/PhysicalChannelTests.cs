using System;
using Bsa.Hardware.Acquisition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Hardware.Tests.Acquisition
{
    [TestClass]
    public class PhysicalChannelTests
    {
        [TestMethod]
        public void CanCreate()
        {
            var channel = new PhysicalChannel(Guid.Empty)
            {
                Name = "Test",
                SamplingRate = 100,
            };

            Assert.AreEqual("Test", channel.Name);
            Assert.AreEqual(100, channel.SamplingRate);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotCreateWithNullName()
        {
            new PhysicalChannel(Guid.Empty) { Name = null };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotCreateWithBlankName1()
        {
            new PhysicalChannel(Guid.Empty) { Name = " " };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotCreateWithBlankName2()
        {
            new PhysicalChannel(Guid.Empty) { Name = "\t" };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CannotCreateWithNegativeSamplingRate()
        {
            new PhysicalChannel(Guid.Empty) { SamplingRate = -1 };
        }
    }
}
