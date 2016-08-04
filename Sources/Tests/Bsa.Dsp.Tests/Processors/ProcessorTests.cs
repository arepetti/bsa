using System;
using Bsa.Dsp.Processors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Dsp.Tests.Processors
{
    [TestClass]
    public class ProcessorTests
    {
        [TestMethod]
        public void UnchangedIfDisabled()
        {
            var inverter = new Inverter();
            inverter.IsEnabled = false;

            Assert.AreEqual(1, inverter.Process(1));
        }

        [TestMethod]
        public void ProcessorInverter()
        {
            Assert.AreEqual(-1, new Inverter().Process(1));
        }

        [TestMethod]
        public void ProcessorRectifier()
        {
            Assert.AreEqual(1, new Inverter().Process(-1));
        }

        [TestMethod]
        public void ProcessorThresholdLessThan()
        {
            var processor = new Threshold() { Type = ThresholdType.LessThan, Level = 10 };
            TestThreshold(processor, -5, -5);
            TestThreshold(processor, 5, 5);
            TestThreshold(processor, 10, 15);
        }

        [TestMethod]
        public void ProcessorThresholdGreaterThan()
        {
            var processor = new Threshold() { Type = ThresholdType.GreaterThan, Level = 10 };
            TestThreshold(processor, 10, -5);
            TestThreshold(processor, 10, 5);
            TestThreshold(processor, 15, 15);
        }

        [TestMethod]
        public void ProcessorThresholdInRange()
        {
            var processor = new Threshold() { Type = ThresholdType.InRange, LowerBoundary = 10, UpperBoundary = 20 };
            TestThreshold(processor, 10, -5);
            TestThreshold(processor, 10, 5);
            TestThreshold(processor, 15, 15);
            TestThreshold(processor, 20, 25);

            processor.Indicator = true;
            Assert.AreEqual(1, processor.Process(10));
            Assert.AreEqual(1, processor.Process(20));
        }

        [TestMethod]
        public void ProcessorThresholdOutOfRange()
        {
            var processor = new Threshold() { Type = ThresholdType.OutOfRange, LowerBoundary = 10, UpperBoundary = 20 };
            TestThreshold(processor, -5, -5);
            TestThreshold(processor, 5, 5);
            TestThreshold(processor, 10, 12);
            TestThreshold(processor, 20, 18);
            TestThreshold(processor, 25, 25);

            processor.Indicator = true;
            Assert.AreEqual(0, processor.Process(10));
            Assert.AreEqual(0, processor.Process(20));
        }

        private static void TestThreshold(Threshold processor, double expectedOutput, double input)
        {
            processor.Indicator = false;
            Assert.AreEqual(expectedOutput, processor.Process(input));

            processor.Indicator = true;
            Assert.AreEqual(expectedOutput == input ? 1 : 0, processor.Process(input));
        }

    }
}
