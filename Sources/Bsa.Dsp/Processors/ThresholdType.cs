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
// along with BSA-F. If not, see <http://www.gnu.org/licenses/>.
//

using System;

namespace Bsa.Dsp.Processors
{
    /// <summary>
    /// Represents the type of threshold applied to input samples by <see cref="Threshold"/> processor.
    /// </summary>
    public enum ThresholdType
    {
        /// <summary>
        /// Changes values that are less than <see cref="Threshold.UpperBoundary"/> to the boundary level, and passes through all other values.
        /// </summary>
        LessThan,

        /// <summary>
        /// Changes values that are greater than <see cref="Threshold.LowerBoundary"/> to the boundary level, and passes through all other values.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Passes through all values that fall within the specified range, and changes values outside the range to the closest boundary value.
        /// </summary>
        InRange,

        /// <summary>
        /// Passes through all values that fall outside the specified range, and changes values inside the range to the closest boundary value.
        /// </summary>
        OutOfRange,
    }
}
