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

                device.Channels.Add(new PhysicalChannel(Guid.NewGuid()) { Name = "1", SamplingRate = 100 });
                device.Channels.Add(new PhysicalChannel(Guid.NewGuid()) { Name = "2", SamplingRate = 100 });
                device.Channels.Seal();

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
        public void CannotSetupWithDuplicatedChannels1()
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
        public void CannotSetupWithDuplicatedChannels2()
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
            using (var device = new TestAcquisitionDeviceWithFirmwareUpdate(false))
            {
                device.Connect();

                Assert.IsFalse(device.HasBeenUpdated, "No firmware update was required but it has been performed.");
            }

            using (var device = new TestAcquisitionDeviceWithFirmwareUpdate(true))
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
