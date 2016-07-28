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
    public sealed class DeviceTests
    {
        [TestMethod]
        public void CanCreateOpenAndDispose()
        {
            using (var device = new TestDevice(0))
            {
                device.Connect();

                Assert.AreEqual(ConnectionState.Connected, device.State);
                Assert.AreEqual(1, device.ConnectionAttempts);
            }
        }

        [TestMethod]
        public void CanMonitorConnectionStatus()
        {
            using (var device = new TestDevice(0))
            {
                int numberOfChanges = 0;

                device.StateChanged += (s, e) => ++numberOfChanges;
                device.Connect();

                Assert.IsTrue(numberOfChanges > 0);
            }
        }

        [TestMethod]
        public void CanConnectTwice()
        {
            using (var device = new TestDevice(0))
            {
                device.Connect();
                device.Connect();

                Assert.AreEqual(ConnectionState.Connected, device.State);
                Assert.AreEqual(1, device.ConnectionAttempts);
            }
        }

        [TestMethod]
        public void CanOpenAndCloseConnection()
        {
            using (var device = new TestDevice(0))
            {
                device.Connect();
                Assert.AreEqual(ConnectionState.Connected, device.State);
                Assert.AreEqual(1, device.ConnectionAttempts);

                device.Disconnect();
                Assert.AreEqual(ConnectionState.Disconnected, device.State);

                device.Connect();
                Assert.AreEqual(ConnectionState.Connected, device.State);
                Assert.AreEqual(2, device.ConnectionAttempts);
            }
        }

        [TestMethod]
        public void RetryConnectionIfFails()
        {
            using (var device = new TestDevice(2))
            {
                var start = DateTime.UtcNow;

                device.Connect();

                Assert.AreEqual(ConnectionState.Connected, device.State);
                Assert.AreEqual(3, device.ConnectionAttempts); // 2 failed + 1 successful

                // At least one second elapsed because of delay between each attempt
                Assert.IsTrue(DateTime.UtcNow - start > TimeSpan.FromSeconds(1));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HardwareException))]
        public void AbortIfConnectionFails()
        {
            using (var device = new TestDevice(Int32.MaxValue))
            {
                device.Connect();
            }
        }

        [TestMethod]
        public void ErrorStateIfConnectionFails()
        {
            using (var device = new TestDevice(Int32.MaxValue))
            {
                try
                {
                    device.Connect();
                }
                catch (HardwareException)
                {
                }

                Assert.AreEqual(ConnectionState.Error, device.State);
            }
        }

        [TestMethod]
        public void CanReconnectAfterError()
        {
            using (var device = new TestDevice(Int32.MaxValue))
            {
                try
                {
                    device.Connect();
                }
                catch (HardwareException)
                {
                }

                Assert.AreEqual(ConnectionState.Error, device.State);

                device.NumberOfConnectionsToFail = 0;
                device.Connect();
                Assert.AreEqual(ConnectionState.Connected, device.State);
            }
        }

        private sealed class TestDevice : NonInstrumentedTestDevice
        {
            public TestDevice(int numberOfConnectionsToFail)
            {
                NumberOfConnectionsToFail = numberOfConnectionsToFail;
            }

            public int ConnectionAttempts
            {
                get;
                private set;
            }

            public int NumberOfConnectionsToFail
            {
                get;
                set;
            }

            protected override void ConnectCore()
            {
                if (++ConnectionAttempts <= NumberOfConnectionsToFail)
                    throw new HardwareException(new HardwareError(HardwareErrorSeverity.Critical, "Test"));
            }
        }
    }
}
