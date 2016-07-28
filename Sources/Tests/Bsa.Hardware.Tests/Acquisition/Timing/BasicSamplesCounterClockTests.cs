//
// This file is part of Biological Signals Acquisition Framework (BSA-F).
// Copyright © Adriano Repetti 2016.
//
// BSA-F is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// BSA-F is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with BSA-F.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using Bsa.Hardware.Acquisition.Timing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Hardware.Tests.Acquisition.Timing
{
    [TestClass]
    public sealed class BasicSamplesCounterClockTests
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

        [TestMethod]
        [ExpectedException(typeof(HardwareException))]
        public void StopsWhenTimestampIsOutOfRange()
        {
            // Sampling rate should be small to enough to fail quickly
            var clock = new SamplesCounterClock(0.000001, SamplesCounterClockOptions.Default);
            while (true)
                clock.Increase(UInt32.MaxValue);
        }
    }
}
