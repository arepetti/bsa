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
    public class TelemetrySessionCountersDiscoveryTests
    {
        [TestMethod]
        public void CanFindDeclaredCounters()
        {
            using (var session = new TestableTelemetrySessionBase())
            {
                Assert.AreEqual(session.RegisteredCounters.Count(), 1);
            }
        }

        [TestMethod]
        public void CanFindCountersInFullTypeHierarchy()
        {
            using (var session = new TestableTelemetrySessionDerived())
            {
                Assert.AreEqual(session.RegisteredCounters.Count(), 2);
            }
        }

        [TestMethod]
        public void CanRegisterCountersOnTheFlyAndDeleteThem()
        {
            try
            {
                using (var session = new TestableTelemetrySessionDerived())
                {
                    Assert.IsFalse(session.Start(), "Required performance counters were already registered, this test has not been performed.");

                    Assert.AreEqual(session.RegisteredCounters.Count(), 2);

                    using (var pc = new PerformanceCounter("BSA.Tests", "Counter1"))
                    {
                        Assert.AreEqual(pc.NextValue(), 0.0f);
                    }

                }
            }
            finally
            {
                WpcTelemetrySessionInstaller.Uninstall(typeof(TestableTelemetrySessionDerived));
            }
        }

        private class TestableTelemetrySessionBase : WpcTelemetrySession
        {
            public static readonly TelemetryData Counter1 = new TelemetryData("BSA.Tests", "Counter1", TelemetryDataType.Count);
        }

        private sealed class TestableTelemetrySessionDerived : TestableTelemetrySessionBase
        {
            public static readonly TelemetryData Counter2 = new TelemetryData("BSA.Tests", "Counter2", TelemetryDataType.Count);
        }
    }
}
