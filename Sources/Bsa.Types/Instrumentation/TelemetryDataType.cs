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

namespace Bsa.Instrumentation
{
    /// <summary>
    /// Specifies the data type of an instrumented value for application telemetry.
    /// </summary>
    /// <remarks>
    /// For details see also: https://msdn.microsoft.com/en-us/library/system.diagnostics.performancecountertype(v=vs.100).aspx.
    /// </remarks>
    public enum TelemetryDataType
    {
        /// <summary>
        /// An instantaneous 32 bit integer counter that shows the most recently observed value.
        /// Used, for example, to maintain a simple count of items or operations.
        /// </summary>
        Count,

        /// <summary>
        /// An average counter that shows how many items are processed, on average, during an operation.
        /// Counters of this type display a ratio of the items processed to the number of operations completed.
        /// The ratio is calculated by comparing the number of items processed during the last interval to the number of operations completed during the last interval.
        /// Formula: (N1 -N0)/(B1 -B0), where N1 and N0 are performance counter readings, and the B1 and B0 are their corresponding AverageBase values.
        /// Thus, the numerator represents the numbers of items processed during the sample interval, and the denominator represents the number of operations completed during the sample interval.
        /// </summary>
        AverageCount,

        /// <summary>
        /// A difference timer that shows the total time between when the component or process started and the time when this value is calculated.
        /// Formula: (D 0 - N 0) / F, where D 0 represents the current time, N 0 represents the time the object was started, and F represents the number of time units that elapse in one second.
        /// The value of F is factored into the equation so that the result can be displayed in seconds.
        /// </summary>
        ElapsedTime
    }
}
