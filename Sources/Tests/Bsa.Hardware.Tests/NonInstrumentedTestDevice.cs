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
// You should have received a copy of the GNU Lesse General Public License
// along with BSA-F.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bsa.Instrumentation;

namespace Bsa.Hardware.Tests
{
    class NonInstrumentedTestDevice : Device
    {
        public NonInstrumentedTestDevice()
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
}
