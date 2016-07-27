using System;
using System.Linq;
using System.Threading;
using Bsa.Examples.Hardware.NoiseGenerator;
using Bsa.Hardware.Acquisition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Hardware.Tests.Acquisition
{
    // Purpose of this class is not to test NoiseGenerator itself but overall architecture
    // exposed by this fictional class. These tests are a complement for AcquisitionDeviceTests.
    // Most of these tests can be used also for basic testing of your own device drivers, copy & paste
    // all relevant code replacing NoiseGeneratorDevice with your own driver class.
    [TestClass]
    public class NoiseGeneratorTests
    {
        [TestMethod]
        public void CanAcquireData()
        {
            using (var device = new NoiseGeneratorDevice())
            {
                ConfigureAndSetupDevice(device);
                int receivedPackets = 0;

                device.Data += delegate(object sender, DataEventArgs e)
                {
                    ValidateDataEventArgs(device, e);
                    
                    ++receivedPackets;
                };

                RunLittleBit(device, AcquisitionMode.Data);
                device.Mode = AcquisitionMode.Idle;

                Assert.IsTrue(receivedPackets > 0, "After some time some data had to be published.");
            }
        }

        [TestMethod]
        public void CanRunCalibration()
        {
            using (var device = new NoiseGeneratorDevice())
            {
                ConfigureAndSetupDevice(device);
                int receivedPackets = 0;

                device.Data += delegate(object sender, DataEventArgs e)
                {
                    // During calibration we should receive "normal" packets 
                    // but with a calibration signal. All we can check is if they're within expected range
                    ValidateDataEventArgs(device, e);

                    Assert.IsTrue(e.Data.Samples.All(channelSamples => channelSamples.All(sample => sample >= MinimumInputValue || sample <= MaximumInputValue)),
                        "Calibration samples must be within expected range");

                    ++receivedPackets;
                };

                RunLittleBit(device, AcquisitionMode.Calibration);
                device.Mode = AcquisitionMode.Idle;

                Assert.IsTrue(receivedPackets > 0, "After some some data had to be published in calibration mode.");
            }
        }

        [TestMethod]
        public void CanReadImpedances()
        {
            using (var device = new NoiseGeneratorDevice())
            {
                ConfigureAndSetupDevice(device);
                int receivedPackets = 0, receivedImpedances = 0;

                device.Data += delegate(object sender, DataEventArgs e)
                {
                    // During impedance measurement we should receive "dummy" packets
                    ValidateDataEventArgs(device, e);

                    ++receivedPackets;
                };

                device.Ohmeter += delegate(object sender, OhmeterEventArgs e)
                {
                    Assert.IsTrue(e.Impedances.All(x => device.Channels.Contains(x.ChannelId)), "All impedances must come from known channels.");
                    Assert.IsTrue(device.Channels.All(channel => e.Impedances.Single(impedance => impedance.ChannelId == channel.Id) != null),
                        "All known channels must have exactly one measured impedance.");

                    Assert.IsTrue(e.Impedances.All(x => x.Impedances.Count <= 1), "All channels for THIS DEVICE must have 0 (unknown impedance?) or 1 (monopolar channel) impedances.");

                    ++receivedImpedances;
                };

                RunLittleBit(device, AcquisitionMode.Ohmeter, 20); // 11 seconds, impedance check is performed every 5 seconds
                device.Mode = AcquisitionMode.Idle;

                Assert.IsTrue(receivedPackets > 0, "After some time some data had to be published in ohmeter mode.");
                Assert.IsTrue(receivedImpedances > 0, "After some time some measures of impedance had to be published.");
            }
        }

        private const double MinimumInputValue = -100;
        private const double MaximumInputValue = 100;

        private static void ConfigureAndSetupDevice(NoiseGeneratorDevice device)
        {
            device.Connect();

            device.Channels.Add(new PhysicalChannel(Guid.NewGuid())
            {
                Name = "I",
                Range = new Range<double>(MinimumInputValue, MaximumInputValue),
                SamplingRate = 128,
            });

            device.Channels.Add(new PhysicalChannel(Guid.NewGuid())
            {
                Name = "II",
                Range = new Range<double>(MinimumInputValue, MaximumInputValue),
                SamplingRate = 128,
            });

            device.Channels.Seal();
            device.Setup();
        }

        private static void ValidateDataEventArgs(NoiseGeneratorDevice device, DataEventArgs e)
        {
            Assert.AreEqual(device.Id, e.Data.AcquisitionDeviceDriverId, "Data must be associated with a driver.");
            Assert.IsTrue(e.Data.Timestamp.HasValue, "Each packet must have a timetamp.");

            Assert.IsTrue(e.Data.Samples != null, "Packet must contain samples.");
            Assert.IsTrue(e.Data.Samples.Length == device.Channels.Count, "Samples must be received for all channels.");
            Assert.IsTrue(e.Data.Samples.All(x => x.Length >= 1), "There must be at least one sample for each channel.");
        }

        private static void RunLittleBit(AcquisitionDevice device, AcquisitionMode mode, int seconds = 2)
        {
            device.Mode = mode;
            Thread.Sleep(seconds * 1000);
        }
    }
}
