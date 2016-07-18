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
using System.ComponentModel;
using System.Diagnostics;

namespace Bsa.Instrumentation
{
    /// <summary>
    /// Represents a value instrumented for application telemetry.
    /// </summary>
    [DebuggerDisplay("{Category}/{Name}")]
    public sealed class TelemetryData : IEquatable<TelemetryData>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TelemetryData"/>.
        /// </summary>
        /// <param name="category">Category for which this meter is associated.</param>
        /// <param name="name">Unique name of this meter.</param>
        /// <param name="type">Type of performance counter, in practice it determines the formula used to calculate each sample.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="category"/> is a <see langword="null"/> or empty string.
        /// <br/>-or-<br/>
        /// If <paramref name="name"/> is a <see langword="null"/> or empty string.
        /// </exception>
        /// <exception cref="InvalidEnumArgumentException">
        /// If <paramref name="type"/> is not a valid <see cref="TelemetryDataType"/> value.
        /// </exception>
        public TelemetryData(string category, string name, TelemetryDataType type)
        {
            if (String.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be a null or blank string.", "category");

            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be a null or blank string.", "name");

            if (!Enum.IsDefined(typeof(TelemetryDataType), type))
                throw new InvalidEnumArgumentException("type", (int)type, typeof(TelemetryDataType));

            Category = category;
            Name = name;
        }

        /// <summary>
        /// Gets the category of this meter.
        /// </summary>
        /// <value>
        /// The category of this meter.
        /// </value>
        public string Category
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of this meter.
        /// </summary>
        /// <value>
        /// The name of this meter.
        /// </value>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of this meter.
        /// </summary>
        /// <value>
        /// The type of this meter, in practice it determines the formula used to calculate each sample.
        /// </value>
        public TelemetryDataType CounterType
        {
            get;
            private set;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.Equals(System.String,System.String,System.StringComparison)", Justification = "Comparison is not 'non-linguistic' but always in English.")]
        public bool Equals(TelemetryData other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;

            return String.Equals(this.Category, other.Category, StringComparison.InvariantCultureIgnoreCase)
                && String.Equals(this.Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Category.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        internal PerformanceCounterType DataTypeToCounterType()
        {
            if (CounterType == TelemetryDataType.ElapsedTime)
                return PerformanceCounterType.ElapsedTime;
            else if (CounterType == TelemetryDataType.AverageCount)
                return PerformanceCounterType.AverageCount64;

            return PerformanceCounterType.NumberOfItems32;
        }
    }
}
