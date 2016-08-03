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
using Bsa.Dsp.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Dsp.Tests.Generators
{
    [TestClass]
    public sealed class PrecomputedSamplesGeneratorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsForNullInput()
        {
            new PrecomputedSamplesGenerator(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ThrowsForInvalidInput()
        {
            new PrecomputedSamplesGenerator(new double[0]);
        }

        [TestMethod]
        public void Loops()
        {
            var generator = new PrecomputedSamplesGenerator(new double[] { 0, 1, 2, 3 });

            Assert.AreEqual(0, generator.Next());
            Assert.AreEqual(1, generator.Next());
            Assert.AreEqual(2, generator.Next());
            Assert.AreEqual(3, generator.Next());

            Assert.AreEqual(0, generator.Next(), "Must loop over input array");
            Assert.AreEqual(1, generator.Next(), "Must loop over input array");
            Assert.AreEqual(2, generator.Next(), "Must loop over input array");
            Assert.AreEqual(3, generator.Next(), "Must loop over input array");

            Assert.AreEqual(0, generator.Next(), "Must loop over input array");
        }

        [TestMethod]
        public void Reset()
        {
            var generator = new PrecomputedSamplesGenerator(new double[] { 0, 1, 2, 3 });
            Assert.AreEqual(0, generator.Next());

            generator.Reset();
            Assert.AreEqual(0, generator.Next());
        }
    }
}
