using System;
using Bsa.Hardware.Acquisition.Timing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Hardware.Tests.Acquisition.Timing
{
    [TestClass]
    public class BasicSamplesCounterClockTests
    {
        [TestMethod]
        public void AutoInitializesReferenceAndCount()
        {
            const int samplingRate = 10;
            var time = DateTimeOffset.UtcNow;

            var clock = new SamplesCounterClock(samplingRate, SamplesCounterClockOptions.Default);
            clock.Increase(samplingRate); // More or less one second after "time"

            Assert.IsTrue(clock.Current > time);
        }

        [TestMethod]
        public void CountsWithGivenReference()
        {
            const int samplingRate = 128;
            var time = DateTimeOffset.UtcNow;

            var clock = new SamplesCounterClock(samplingRate, SamplesCounterClockOptions.Default);
            clock.Reference = time;
            clock.Increase(samplingRate);

            Assert.AreEqual(time.AddSeconds(1), clock.Current);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsForWrongSamplingRate()
        {
            new SamplesCounterClock(-1, SamplesCounterClockOptions.Default);
        }

        [TestMethod]
        [ExpectedException(typeof(HardwareException))]
        public void ThrowsIfCannotGuaranteePrecision()
        {
            new SamplesCounterClock(Double.MaxValue, SamplesCounterClockOptions.Default);
        }

        [TestMethod]
        public void CanAutoResetOnOverflow()
        {
            // Very high sampling frequency otherwise we will exhaust DateTime range before we reach MaximumNumberOfSamples
            var clock = new SamplesCounterClock(1000000,
                SamplesCounterClockOptions.AdjustForOverflow | SamplesCounterClockOptions.ForceMonotonic);

            // Force creations of reference time
            var time = clock.Current;

            // Enqueue enough samples to cause an overflow
            DateTimeOffset latestPacketTime = DateTimeOffset.MinValue;
            for (long i = 0; i <= (SamplesCounterClock.MaximumNumberOfSamples + 1) / UInt32.MaxValue; ++i)
            {
                var thisPacketTime = clock.Increase(UInt32.MaxValue);
                Assert.IsTrue(thisPacketTime > latestPacketTime);

                latestPacketTime = thisPacketTime;
            }

            // Now reference time has been "regenerated"
            Assert.AreNotEqual(time, clock.Reference.Value);
        }
    }
}
