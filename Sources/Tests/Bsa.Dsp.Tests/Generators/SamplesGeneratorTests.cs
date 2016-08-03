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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bsa.Dsp.Generators;

namespace Bsa.Dsp.Tests.Generators
{
    [TestClass]
    public sealed class SamplesGeneratorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsForNullInput()
        {
            var generator = CreateGeneratorWithConstantValue();
            generator.Next(null, 0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsForInvalidInput1()
        {
            var generator = CreateGeneratorWithConstantValue();
            generator.Next(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsForInvalidInput2()
        {
            var generator = CreateGeneratorWithConstantValue();
            generator.Next(new double[] { 1 }, -1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsForInvalidInput3()
        {
            var generator = CreateGeneratorWithConstantValue();
            generator.Next(new double[] { 1 }, 0, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsForInvalidInput4()
        {
            var generator = CreateGeneratorWithConstantValue();
            generator.Next(new double[] { 1 }, 1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ThrowsForInvalidInput5()
        {
            var generator = CreateGeneratorWithConstantValue();
            generator.Next(new double[] { 1 }, 0, 2);
        }

        [TestMethod]
        public void GeneratesArray()
        {
            var generator = CreateGeneratorWithConstantValue();
            Assert.IsTrue(generator.Next(NumberOfSamplesToGenerateForTesting).All(x => x == ConstantValue));
        }

        [TestMethod]
        public void FillsArray()
        {
            const double anotherValue = ConstantValue + 1;

            var generator = CreateGeneratorWithConstantValue();
            double[] array = new double[] { anotherValue, anotherValue, anotherValue, anotherValue };
            generator.Next(array, 1, 2);

            Assert.AreEqual(array[0], anotherValue);
            Assert.AreEqual(array[1], ConstantValue);
            Assert.AreEqual(array[2], ConstantValue);
            Assert.AreEqual(array[3], anotherValue);
        }

        [TestMethod]
        public void CanLoopEndless()
        {
            var generator = CreateGeneratorWithConstantValue();
            int generatedCount = 0;
            foreach (var sample in generator.Infinite())
            {
                if (++generatedCount == NumberOfSamplesToGenerateForTesting)
                    break;

                Assert.AreEqual(sample, ConstantValue);
            }
        }

        private const double ConstantValue = 100;
        private const int NumberOfSamplesToGenerateForTesting = 1000;

        private static SamplesGenerator CreateGeneratorWithConstantValue()
        {
            return new PrecomputedSamplesGenerator(new double[] { ConstantValue });
        }
    }
}
