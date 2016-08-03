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
// along with BSA-F. If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Linq;
using Bsa.Dsp.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Dsp.Tests.Generators
{
    [TestClass]
    public sealed class WaveGeneratorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsIfNullArgument()
        {
            new WaveGenerator(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsIfInvalidSamplingRate()
        {
            new WaveformDescription { SamplingRate = WaveformDescription.MaximumSamplingRate + 1 };
        }

        [TestMethod]
        public void GeneratesSine()
        {
            const int samplingRate = 10;
            const double offset = 10, frequency = 1, amplitude = 1;

            var generator = new WaveGenerator(new WaveformDescription
            {
                Waveform = Waveform.Sine,
                SamplingRate = samplingRate,
                Frequency = frequency,
                Amplitude = amplitude,
                Offset = offset,
                Phase = 0,
            });

            double[] samples = generator.Next(samplingRate);

            Assert.AreEqual(samplingRate, samples.Length, "Expected one second of generated data.");
            Assert.AreEqual(offset, samples.Average(), "Mean value must be equal to offset.");
            Assert.IsTrue(samples.All(x => x >= offset - amplitude && x <= offset + amplitude), "Samples must be within expected range.");
        }

        [TestMethod]
        public void GeneratesDc()
        {
            const int samplingRate = 10;
            const double offset = 10, amplitude = 1;

            var generator = new WaveGenerator(new WaveformDescription
            {
                Waveform = Waveform.Dc,
                SamplingRate = samplingRate,
                Amplitude = amplitude,
                Offset = offset,
            });

            double[] samples = generator.Next(samplingRate);

            Assert.AreEqual(samplingRate, samples.Length, "Expected one second of generated data.");
            Assert.IsTrue(samples.All(x => Math.Abs(x - (offset + amplitude)) <= Double.Epsilon), "Samples must have expected value.");
        }

        [TestMethod]
        public void GeneratesImpulse()
        {
            const int samplingRate = 10;
            const double offset = 10, amplitude = 1;

            var generator = new WaveGenerator(new WaveformDescription
            {
                Waveform = Waveform.Impulse,
                SamplingRate = samplingRate,
                Amplitude = amplitude,
                Offset = offset,
                Phase = WaveformDescription.Phase180
            });

            double[] samples = generator.Next(samplingRate);

            Assert.AreEqual(samplingRate, samples.Length, "Expected one second of generated data.");
            
            for (int i=0; i < samples.Length; ++i)
                Assert.AreEqual(i == samplingRate / 2 ? amplitude + offset : offset, samples[i]);
        }
    }
}
