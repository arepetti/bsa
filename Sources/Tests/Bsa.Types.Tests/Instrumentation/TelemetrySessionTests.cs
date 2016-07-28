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
using System.Diagnostics;
using System.Linq;
using Bsa.Instrumentation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Tests.Instrumentation
{
    [TestClass]
    public sealed class TelemetrySessionTests
    {
        [TestInitialize]
        public void Initialize()
        {
            WpcTelemetrySessionInstaller.Install(typeof(TestableTelemetrySession));
        }

        [TestCleanup]
        public void Cleanup()
        {
            WpcTelemetrySessionInstaller.Uninstall(typeof(TestableTelemetrySession));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotUseUnregisteredCounters()
        {
            using (var session = new TestableTelemetrySession())
            {
                session.Start();
                session.Increment(new TelemetryData("BSA.Tests", "Unexisting", TelemetryDataType.ElapsedTime), 1);
            }
        }

        [TestMethod]
        public void CanIncrementCounter()
        {
            using (var session = new TestableTelemetrySession())
            {
                // Before Start() this increment will be ignored
                session.Increment(TestableTelemetrySession.Counter1, 1);

                session.Start();

                session.Increment(TestableTelemetrySession.Counter1, 1);
                Assert.AreEqual((int)ReadPerformanceCounter("BSA.Tests", "Counter1"), 1);

                session.Increment(TestableTelemetrySession.Counter1, 1);
                Assert.AreEqual((int)ReadPerformanceCounter("BSA.Tests", "Counter1"), 2);

                session.Increment(TestableTelemetrySession.Counter1, 2);
                Assert.AreEqual((int)ReadPerformanceCounter("BSA.Tests", "Counter1"), 4);
            }
        }

        private float ReadPerformanceCounter(string category, string name)
        {
            using (var pc = new PerformanceCounter(category, name))
            {
                return pc.NextValue();
            }
        }

        private class TestableTelemetrySession : WpcTelemetrySession
        {
            public static readonly TelemetryData Counter1 = new TelemetryData("BSA.Tests", "Counter1", TelemetryDataType.Count);
        }
    }
}
