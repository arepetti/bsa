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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Bsa.Instrumentation
{
    /// <summary>
    /// Represents the object which collect telemetry data and stores it in Windows Performance Counters.
    /// </summary>
    /// <remarks>
    /// Windows Performance Counters should be registered during application installation (not at run-time).
    /// Run-time registration is available as "safe net" but their usage may be broken and/or they may not register any data
    /// until OS cache is refreshed. Declare your own <c>System.Configuration.Install.Installer</c> class and call
    /// <see cref="WpcTelemetrySessionInstaller.Install"/> and <see cref="WpcTelemetrySessionInstaller.Uninstall"/> during installation and uninstallation.
    /// <br/>
    /// Derived classes should declare their own meters as <c>static readonly</c> fields of type <see cref="TelemetryData"/>,
    /// all fields declared in type hierarchy will be available to collect instrumentation data.
    /// </remarks>
    public abstract class WpcTelemetrySession : TelemetrySession
    {
        /// <summary>
        /// Creates a new instance of <see cref="WpcTelemetrySession"/>.
        /// </summary>
        protected WpcTelemetrySession()
        {
            _telemetryData = new PerformanceCounterHolderCollection(FindTelemetryData(GetType()));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override IEnumerable<TelemetryData> RegisteredCounters
        {
            get { return _telemetryData.Data; }
        }

        public override bool Start()
        {
            ThrowIfDisposed();

            _started = true;

            // Calling CreatePerformanceCounters() here is "just in case" application has not been properly
            // installed, PCs should be registered in advance during installation, if OS didn't refresh its lists
            // we will not be able to use them for a while.
            return !WpcTelemetrySessionInstaller.RegisterPerformanceCounters(_telemetryData.Data);
        }

        public override void Stop()
        {
            ThrowIfDisposed();

            _started = false;
        }

        public override void Increment(TelemetryData data, int amount)
        {
            ThrowIfDisposed();

            if (!_started)
                return;

            if (data == null)
                throw new ArgumentNullException("data");

            if (data.CounterType == TelemetryDataType.ElapsedTime)
                throw new ArgumentException("Cannot use Increment() with TelemetryDataType.ElapsedTime, use StopwatchToggle() instead.");

            // This restriction (TelmetryData must be declared in a derived class) effectively limits our ability to change telemetry storage
            // because a derived class is defined (for example Bsa.Hardware.DeviceTelemetry) then a more derived class cannot change its storage
            // without breaking all the code the used declared counters in base class. If it will ever be an issue then this restriction may be relaxed
            // or declaration and storage may be further decoupled.
            PerformanceCounterHolder holder;
            if (!_telemetryData.TryGetValue(data, out holder))
                throw new ArgumentException("Unknown telemetry data, only data declared in this session can be used.", "data");

            if (holder.IsDisabled)
                return;

            holder.Counter.IncrementBy(amount);
            if (data.CounterType == TelemetryDataType.AverageCount)
                holder.Base.Increment();
        }

        public override void ToggleStopwatch(TelemetryData data)
        {
            ThrowIfDisposed();

            if (!_started)
                return;

            if (data == null)
                throw new ArgumentNullException("data");

            if (data.CounterType != TelemetryDataType.ElapsedTime)
                throw new ArgumentException("You can use StopwatchToggle() only with TelemetryDataType.ElapsedTime, use Increment() for the other counters.");

            // This restriction (TelmetryData must be declared in a derived class) effectively limits our ability to change telemetry storage
            // because a derived class is defined (for example Bsa.Hardware.DeviceTelemetry) then a more derived class cannot change its storage
            // without breaking all the code the used declared counters in base class. If it will ever be an issue then this restriction may be relaxed
            // or declaration and storage may be further decoupled.
            PerformanceCounterHolder holder;
            if (!_telemetryData.TryGetValue(data, out holder))
                throw new ArgumentException("Unknown telemetry data, only data declared in this session can be used.", "data");

            if (holder.IsDisabled)
                return;

            holder.Counter.RawValue = Stopwatch.GetTimestamp();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed)
                    ((IDisposable)_telemetryData).Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private bool _started;
        private readonly PerformanceCounterHolderCollection _telemetryData;
    }
}
