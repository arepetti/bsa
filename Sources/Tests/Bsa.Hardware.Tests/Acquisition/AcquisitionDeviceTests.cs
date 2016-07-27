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
using Bsa.Instrumentation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Hardware.Tests.Acquisition
{
    [TestClass]
    public class AcquisitionDeviceTests
    {
        [TestMethod]
        public void CanSetupDevice()
        {
            using (var device = new TestAcquisitionDevice())
            {
                device.Connect();

                var id1 = Guid.NewGuid();
                var id2 = Guid.NewGuid();

                device.Channels.Add(new PhysicalChannel(id1) { Name = "1", SamplingRate = 100 });
                device.Channels.Add(new PhysicalChannel(id2) { Name = "2", SamplingRate = 100 });
                device.Channels.Seal();

                // These are basic tests on PhysicalChannelCollection. If they grow in number
                // then it may be worth to move to a separate class.
                Assert.IsTrue(device.Channels.Contains(id1));
                Assert.IsTrue(device.Channels.Contains(id2));
                Assert.IsFalse(device.Channels.Contains(Guid.NewGuid()));
                Assert.IsTrue(device.Channels[id1].Id == id1);

                device.Setup();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HardwareException))]
        public void CannotSetupBeforeConnection()
        {
            using (var device = new TestAcquisitionDevice())
            {
                device.Setup();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HardwareException))]
        public void CannotSetupWithoutChannels()
        {
            using (var device = new TestAcquisitionDevice())
            {
                device.Connect();
                device.Setup();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HardwareException))]
        public void ByDefaultCannotSetupForMultifrequency()
        {
            using (var device = new TestAcquisitionDevice())
            {
                device.Connect();

                device.Channels.Add(new PhysicalChannel(Guid.NewGuid()) { Name = "1", SamplingRate = 100 });
                device.Channels.Add(new PhysicalChannel(Guid.NewGuid()) { Name = "2", SamplingRate = 200 });

                device.Setup();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HardwareException))]
        public void CannotSetupWithDuplicatedChannelIds()
        {
            using (var device = new TestAcquisitionDevice())
            {
                device.Connect();

                device.Channels.Add(new PhysicalChannel(Guid.Empty) { Name = "1", SamplingRate = 100 });
                device.Channels.Add(new PhysicalChannel(Guid.Empty) { Name = "2", SamplingRate = 100 });

                device.Setup();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HardwareException))]
        public void CannotSetupWithDuplicatedChannelNames()
        {
            using (var device = new TestAcquisitionDevice())
            {
                device.Connect();

                device.Channels.Add(new PhysicalChannel(Guid.NewGuid()) { Name = "1", SamplingRate = 100 });
                device.Channels.Add(new PhysicalChannel(Guid.NewGuid()) { Name = "1", SamplingRate = 100 });

                device.Setup();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HardwareException))]
        public void MustSealChannels()
        {
            using (var device = new TestAcquisitionDevice())
            {
                device.Connect();

                device.Channels.Add(new PhysicalChannel(Guid.NewGuid()) { Name = "1", SamplingRate = 100 });

                device.Setup();
            }
        }

        [TestMethod]
        public void CanUpdateFirmwareIfRequired()
        {
            using (var device = new TestAcquisitionDeviceWithFirmwareUpdate(canUpdate: false))
            {
                device.Connect();

                Assert.IsFalse(device.HasBeenUpdated, "No firmware update was required but it has been performed.");
            }

            using (var device = new TestAcquisitionDeviceWithFirmwareUpdate(canUpdate: true))
            {
                device.Connect();

                Assert.IsTrue(device.HasBeenUpdated, "Firmware update was required but it has not been performed.");
            }
        }

        class TestAcquisitionDevice : AcquisitionDevice<PhysicalChannel>
        {
            public TestAcquisitionDevice()
                : base(null)
            {
            }

            protected override void ConnectCore()
            {
            }

            protected override void DisconnectCore()
            {
            }

            protected override TelemetrySession CreateSession()
            {
                // We wrap telemetry into NullTelemetrySession, in these tests we're not interested
                // in telemetry and if counters must be installed it may slow down execution.
                return new NullTelemetrySession(base.CreateSession());
            }
        }

        sealed class TestAcquisitionDeviceWithFirmwareUpdate : TestAcquisitionDevice
        {
            public TestAcquisitionDeviceWithFirmwareUpdate(bool canUpdate)
            {
                _canUpdate = canUpdate;
            }

            public bool HasBeenUpdated
            {
                get;
                private set;
            }

            public bool IsFeatureFirmwareUpdateAvailable()
            {
                return true;
            }

            public bool IsFeatureFirmwareUpdateEnabled()
            {
                return _canUpdate;
            }

            public bool PerformFirmwareUpdate()
            {
                HasBeenUpdated = true;

                return true;
            }

            private readonly bool _canUpdate;
        }
    }
}
