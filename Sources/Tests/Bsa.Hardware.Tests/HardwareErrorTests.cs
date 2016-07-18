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
using Bsa.Instrumentation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Hardware.Tests
{
    [TestClass]
    public class HardwareErrorTests
    {
        [TestMethod]
        public void CanCreateType()
        {
            Assert.AreEqual(new HardwareError(HardwareErrorSeverity.Critical, "Test").Message, "Test");

            Assert.AreEqual(HardwareErrorSeverity.Critical, new HardwareError(HardwareErrorSeverity.Critical, HardwareErrorClass.Device, 1, "Test").Severity);
            Assert.AreEqual(HardwareErrorClass.Device, new HardwareError(HardwareErrorSeverity.Critical, HardwareErrorClass.Device, 1, "Test").Class);
            Assert.AreEqual(1, new HardwareError(HardwareErrorSeverity.Critical, HardwareErrorClass.Device, 1, "Test").Code);
            Assert.AreEqual("Test", new HardwareError(HardwareErrorSeverity.Critical, HardwareErrorClass.Device, 1, "Test").Message);
        }

        [TestMethod]
        public void IsHResultCorrect()
        {
            Assert.AreEqual(0xA0000000, new HardwareError(HardwareErrorSeverity.Critical, HardwareErrorClass.Generic, 0, "Test").ToHResult());
            Assert.AreEqual(0xA0010001, new HardwareError(HardwareErrorSeverity.Critical, HardwareErrorClass.Internal, 1, "Test").ToHResult());
        }

        [TestMethod]
        public void ExceptionMessageIsFirstErrorMessage()
        {
            var error1 = new HardwareError(HardwareErrorSeverity.Critical, HardwareErrorClass.Device, 1, "1");
            var error2 = new HardwareError(HardwareErrorSeverity.Critical, HardwareErrorClass.Device, 2, "2");
            var exception = new HardwareException(new HardwareError[] { error1, error2 });

            Assert.AreEqual(error1.Message, exception.Message);
        }
    }
}
