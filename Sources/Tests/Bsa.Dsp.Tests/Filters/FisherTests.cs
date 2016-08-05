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
using Bsa.Dsp.Filters;
using Bsa.Dsp.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Dsp.Tests.Filters
{
    [TestClass]
    public class FisherTests
    {
        [TestMethod]
        public void Chebyshev()
        {
            TestFilter(OnlineFilterDesign.Iir.ChebyshevI);
        }

        [TestMethod]
        public void Butterworth()
        {
            TestFilter(OnlineFilterDesign.Iir.Butterworth);
        }

        [TestMethod]
        public void Bessel()
        {
            TestFilter(OnlineFilterDesign.Iir.Bessel);
        }

        private const int SamplingRate = 128;
        private const int SampleLengthInSeconds = 5;

        private static void TestFilter(OnlineFilterDesigner designer)
        {
            var samples = CreateTestSamples();

            var lowPass = OnlineFilterFactory.Create(FilterKind.LowPass, designer, CreateSettings(), SamplingRate / 4);
            AssertSomethingChanged(samples, lowPass.Process(samples));

            var highPass = OnlineFilterFactory.Create(FilterKind.HighPass, designer, CreateSettings(), SamplingRate / 4);
            AssertSomethingChanged(samples, highPass.Process(samples));

            var notch = OnlineFilterFactory.Create(FilterKind.Notch, designer, CreateSettings(), SamplingRate / 4);
            AssertSomethingChanged(samples, notch.Process(samples));

            var bandPass = OnlineFilterFactory.Create(FilterKind.BandPass, designer, CreateSettings(), new Range<double>(SamplingRate / 4 - 10, SamplingRate / 4 + 10));
            AssertSomethingChanged(samples, bandPass.Process(samples));

            var bandStop = OnlineFilterFactory.Create(FilterKind.BandStop, designer, CreateSettings(), new Range<double>(SamplingRate / 4 - 10, SamplingRate / 4 + 10));
            AssertSomethingChanged(samples, bandStop.Process(samples));
        }

        private static FilterDesignSettings CreateSettings()
        {
            return new FilterDesignSettings
            {
                Order = 2,
                SamplingRate = SamplingRate
            };
        }

        private static double[] CreateTestSamples()
        {
            var rnd = WhiteGaussianNoiseGenerator.CreateDefaultRandomGenerator();
            var generator = new WhiteGaussianNoiseGenerator(rnd);
            
            return generator.Next(SamplingRate * SampleLengthInSeconds);
        }

        // TODO: A decent test should at least verify frequency spectrum changed as expected,
        // this is just a very basic check waiting to implement FFT
        private static void AssertSomethingChanged(double[] unfiltered, double[] filtered)
        {
            Assert.IsNotNull(filtered);
            Assert.AreEqual(unfiltered.Length, filtered.Length);

            for (int i = 0; i < unfiltered.Length; ++i)
            {
                if (unfiltered[i] != filtered[i])
                    return;
            }

            Assert.Fail("Filtered and unfiltered buffers contains exactly the same values.");
        }
    }
}
