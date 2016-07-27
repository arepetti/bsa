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
