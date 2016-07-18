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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Bsa.Instrumentation
{
    /// <summary>
    /// Represents the object which collect telemetry data and stores/sends it to its storage.
    /// </summary>
    /// <remarks>
    /// Derived classes should declare their own meters as <c>static readonly</c> fields of type <see cref="TelemetryData"/>,
    /// all fields declared in type hierarchy will be available to collect instrumentation data.
    /// </remarks>
    public abstract class TelemetrySession : Disposable
    {
        /// <summary>
        /// Gets the list of registered counters for this session.
        /// </summary>
        /// <value>
        /// The list of registered counters for this session. The list contains only counters found on the
        /// derived class and it doesn't consider if these counters are effectively available in the telemetry storage
        /// (for example Windows Performance Counters may not be available yet).
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract IEnumerable<TelemetryData> RegisteredCounters
        {
            get;
        }

        /// <summary>
        /// Starts collecting telemetry data. Data can be sent to this object after collection has been started.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if it's safe to immediately start collecting telemetry data, if 
        /// if required performance counters have not been registered this method tries to register them on-the-fly. Just registered
        /// counters may not work and application should be at least restarted, in this case this function returns <see langword="false"/>.
        /// </returns>
        public abstract bool Start();

        /// <summary>
        /// Stop collecting telemetry data. All data sent to this object will be silently ignored.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Stop is a Visual Basic keyword, VB support is not planned.")]
        public abstract void Stop();

        /// <summary>
        /// Increment the value of the specified counter.
        /// </summary>
        /// <param name="data">The counter that should be incremented by the given value.</param>
        /// <param name="amount">Amount of the increment.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="data"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="data"/> is of type <see cref="TelemetryDataType.ElapsedTime"/> (<see cref="ToggleStopwatch"/> should be used instead).
        /// <br/>-or-<br/>
        /// If <paramref name="data"/> is not a counter registered for this object.
        /// </exception>
        /// <remarks>
        /// If collection has not been started, it's stopped or specified counter is not accessible in the current data storage then
        /// value is simply silently discarded.
        /// </remarks>
        public abstract void Increment(TelemetryData data, int amount);

        /// <summary>
        /// Start/stop collecting data for a counter that measures elapsed time to complete an operation.
        /// </summary>
        /// <param name="data">The counter that stores information about elapsed time to complete an operation.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="data"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="data"/> is not of type <see cref="TelemetryDataType.ElapsedTime"/> (<see cref="Increment"/> should be used instead).
        /// <br/>-or-<br/>
        /// If <paramref name="data"/> is not a counter registered for this object.
        /// </exception>
        /// <remarks>
        /// If collection has not been started, it's stopped or specified counter is not accessible in the current data storage then
        /// value is simply silently discarded.
        /// </remarks>
        public abstract void ToggleStopwatch(TelemetryData data);

        /// <summary>
        /// Finds all the telemetry data declared in the specified type.
        /// </summary>
        /// <param name="type">
        /// The type in which you want to search for telemetry data. Telemetry data is declared as <see langword="static"/> and <see langword="readonly"/>
        /// public fields with type <see cref="TelemetryData"/>.
        /// </param>
        /// <returns>
        /// All the telemetry data declared in specified type and in all its base classes.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="type"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="type"/> is not a class derived from <see cref="TelemetrySession"/>.
        /// </exception>
        protected internal static TelemetryData[] FindTelemetryData(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!typeof(TelemetrySession).IsAssignableFrom(type))
                throw new ArgumentException("Type must be derived from TelemetrySession.", "type");

            return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(x => typeof(TelemetryData).IsAssignableFrom(x.FieldType) && x.Attributes.HasFlag(FieldAttributes.InitOnly))
                .Select(x => (TelemetryData)x.GetValue(null))
                .ToArray();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed)
                    Stop();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
