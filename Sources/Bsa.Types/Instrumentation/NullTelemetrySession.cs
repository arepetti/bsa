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
using System.Linq;
using System.Collections.Generic;

namespace Bsa.Instrumentation
{
    /// <summary>
    /// Empty implementation of <see cref="TelemetrySession"/>.
    /// </summary>
    /// <remarks>
    /// This implementation does not perform any storage operation but it validates its inputs.
    /// </remarks>
    public sealed class NullTelemetrySession : TelemetrySession
    {
        /// <summary>
        /// Initializes a new instance of <see cref="NullTelemetrySession"/>.
        /// </summary>
        /// <param name="sessionToFake">
        /// The type of the class derived from <see cref="TelemetrySession"/> that must be
        /// <em>faked</em> by this <em>null</em> telemetry session object. This class will accept all
        /// counters declared in the specified type but no operation will be actually performed. Specify
        /// <see langword="null"/> (use <c>new NullTelemetrySession((Type)null)</c>) to obtain a <see cref="NullTelemetrySession"/>
        /// without accepted counters (it will result in errors if you try to use <see cref="Increment"/>
        /// or <see cref="ToggleStopwatch"/> methods).
        /// </param>
        public NullTelemetrySession(Type sessionToFake)
        {
            _telemetryData = FindTelemetryData(sessionToFake ?? GetType());
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NullTelemetrySession"/>.
        /// </summary>
        /// <param name="sessionToFake">
        /// An object derived from <see cref="TelemetrySession"/> that must be
        /// <em>faked</em> by this <em>null</em> telemetry session object. This class will accept all
        /// counters declared in the specified type but no operation will be actually performed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="sessionToFake"/> is <see langword="null"/>
        /// </exception>
        public NullTelemetrySession(TelemetrySession sessionToFake)
        {
            if (sessionToFake == null)
                throw new ArgumentNullException("sessionToFake");

            _telemetryData = FindTelemetryData(sessionToFake.GetType());
        }

        public override IEnumerable<TelemetryData> RegisteredCounters
        {
            get { return _telemetryData; }
        }

        public override bool Start()
        {
            ThrowIfDisposed();

            return _started = true;
        }

        public override void Stop()
        {
            ThrowIfDisposed();

            _started = false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is done in ThrowIfInvalidData")]
        public override void Increment(TelemetryData data, int amount)
        {
            ThrowIfInvalidData(data);

            if (data.CounterType == TelemetryDataType.ElapsedTime)
                throw new ArgumentException("Cannot use Increment() with TelemetryDataType.ElapsedTime, use StopwatchToggle() instead.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is done in ThrowIfInvalidData")]
        public override void ToggleStopwatch(TelemetryData data)
        {
            ThrowIfInvalidData(data);

            if (data.CounterType != TelemetryDataType.ElapsedTime)
                throw new ArgumentException("Cannot use Increment() with TelemetryDataType.ElapsedTime, use Increment() instead.");
        }

        private bool _started;
        private readonly TelemetryData[] _telemetryData;

        private void ThrowIfInvalidData(TelemetryData data)
        {
            ThrowIfDisposed();

            if (!_started)
                return;

            if (data == null)
                throw new ArgumentNullException("data");

            if (!_telemetryData.Contains(data))
                throw new ArgumentException("Unknown telemetry data, only data declared in this session can be used.", "data");
        }
    }
}
