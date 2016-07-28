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

namespace Bsa.Hardware.Acquisition.Timing
{
    /// <summary>
    /// Specifies the properties of a timestamp generator.
    /// </summary>
    [Flags]
    public enum TimestampGeneratorProperties
    {
        /// <summary>
        /// Generator has not any special property.
        /// </summary>
        None = 0,

        /// <summary>
        /// Generator is a (strictly) monotonically increasing function.
        /// </summary>
        /// <remarks>
        /// Function is monotonically increasing in respect to multiple calls to <see cref="TimestampGenerator.Increase"/>,
        /// and strictly monotonically increasing for calls where <c>numberOfSamples</c> parameter of that function is greater than zero.
        /// </remarks>
        Monotonic = 1,

        /// <summary>
        /// Generated values are uniformally distributed over time respct each acquired sample.
        /// </summary>
        /// <remarks>
        /// Function is considered uniform without considering numerical errors and error defined in <see cref="TimestampGenerator.Error"/>,
        /// also this property doesn't limit the usage of <see cref="TimestampGenerator.Increase"/> forcing to always have the same value
        /// for <c>numberOfSamples</c>. An uniform distribution of time means that distance between each sample is always the same (given
        /// above preconditions).
        /// </remarks>
        UniformlyDistributed = 2,
    }
}
