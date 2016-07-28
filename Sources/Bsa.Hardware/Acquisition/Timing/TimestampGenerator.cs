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
using System.ComponentModel;

namespace Bsa.Hardware.Acquisition.Timing
{
    /// <summary>
    /// Base class of timestamp generators, used to determine a reliable timestamp for acquired samples and <see cref="DataPacket"/>.
    /// </summary>
    public abstract class TimestampGenerator
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TimestampGenerator"/>.
        /// </summary>
        /// <param name="properties">Properties of this generator.</param>
        protected TimestampGenerator(TimestampGeneratorProperties properties)
        {
            _properties = properties;
        }

        /// <summary>
        /// Gets the properties of this generator.
        /// </summary>
        /// <value>
        /// The properties of this generator. Users of this class may determine behavior and properties (for example if
        /// generator is a monotonically increasing function) inspecting this class. This value will not change then
        /// it also may be used to <em>validate</em> a generator during initialization.
        /// </value>
        public TimestampGeneratorProperties Properties
        {
            get { return _properties; }
        }

        /// <summary>
        /// Gets the current timestamp.
        /// </summary>
        /// <value>
        /// Timestamp of the last acquired sample.
        /// </value>
        public abstract DateTimeOffset Current
        {
            get;
        }

        /// <summary>
        /// Gets/sets the maximum error (in ticks) of this generator.
        /// </summary>
        /// <value>
        /// The maximum error (either positive or negative) expressed in ticks for this generator.
        /// This number is usually between (-1..+1) and it's the maximum error of <see cref="Current"/> in
        /// any time instant. Note that error may be not evenly distributed over time and that derived classes
        /// may introduce different errors during calculation. Not every implementation can calculate this value,
        /// in those cases <c>Error</c> shall be 0.
        /// </value>
        public double Error
        {
            get;
            protected set;
        }

        /// <summary>
        /// Increase the counter used to determine the <see cref="Current"/> time.
        /// </summary>
        /// <param name="numberOfSamples">
        /// Number of newly acquired samples, current time will be adjusted to match the timestamp of last acquired sample.
        /// </param>
        /// <returns>
        /// Timestamp of the first sample of the newly acquired samples <paramref name="numberOfSamples"/>. This value is effectively
        /// the value of <c>Current</c> + 1 sample before calling this function.
        /// </returns>
        public abstract DateTimeOffset Increase(uint numberOfSamples);

        private readonly TimestampGeneratorProperties _properties;
    }
}
