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
using Bsa.Instrumentation;

namespace Bsa.Hardware
{
    /// <summary>
    /// Contains the definition of the values instrumented for hardware telemetry.
    /// </summary>
    public class DeviceTelemetry : WpcTelemetrySession
    {
        public static readonly TelemetryData NumberOfSuccessfulConnections = new TelemetryData("BSA.Hardware.Device", "NumberOfSuccessfulConnections", TelemetryDataType.Count);
        public static readonly TelemetryData NumberOfFailedConnections = new TelemetryData("BSA.Hardware.Device", "NumberOfFailedConnections", TelemetryDataType.Count);
        public static readonly TelemetryData NumberOfErrors = new TelemetryData("BSA.Hardware.Device", "NumberOfErrors", TelemetryDataType.Count);
    }
}
