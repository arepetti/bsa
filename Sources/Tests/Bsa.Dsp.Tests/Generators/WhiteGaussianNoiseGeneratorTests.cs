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
    public sealed class WhiteGaussianNoiseGeneratorTests
    {
        [TestMethod]
        public void ResultsAreReal()
        {
            Random rnd = new Random();
            var generator = new WhiteGaussianNoiseGenerator(() => rnd.NextDouble());
            generator.Range = new Range<double>(-1, 1);

            Assert.IsFalse(generator.Next(1000).Any(x => Double.IsNaN(x) || Double.IsInfinity(x)));
        }

        [TestMethod]
        public void MeanSignalPowerIsOne()
        {
            // We're working with random numbers then our test is little bit aleatory,
            // it's better to repeat it few times to be sure it didn't pass "by case".
            // Also the number of samples MUST be high enough to have a reliable distribution.
            const int numberOfSamples = 100000;
            const int repetitions = 5;

            Random rnd = new Random();
            for (int repetition = 0; repetition < repetitions; ++repetition)
            {
                var generator = new WhiteGaussianNoiseGenerator(() => rnd.NextDouble());
                generator.Range = new Range<double>(-1, 1);

                double power = Enumerable.Range(0, numberOfSamples)
                    .Sum(x => Math.Pow(Magnitude(generator.Next()), 2.0)) / numberOfSamples;

                Assert.AreEqual(1.0, power, 0.1);
            }
        }

        [TestMethod]
        public void SpectrumIsWellDistributed()
        {
            // TODO
        }

        [TestMethod]
        public void SpectralPowerMatchesSignalPower()
        {
            // TODO
        }

        private static double Magnitude(double r)
        {
            return Math.Abs(r);
        }
    }
}
