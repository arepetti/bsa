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

namespace Bsa
{
    /// <summary>
    /// Represents a range.
    /// </summary>
    /// <typeparam name="T">The type of the value for range limits.</typeparam>
    [Serializable]
    public struct Range<T>
        where T : struct
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Range{T}"/> with the given interval.
        /// </summary>
        /// <param name="minimum">Minimum inclusive value of the range.</param>
        /// <param name="maximum">Maximum inclusive value of the range.</param>
        public Range(T minimum, T maximum)
        {
            _minimum = minimum;
            _maximum = maximum;
        }

        /// <summary>
        /// Gets the minimum inclusive value of this range.
        /// </summary>
        /// <value>
        /// The minimum inclusive value of this range.
        /// </value>
        /// <seealso cref="Maximum"/>
        public T Minimum
        {
            get { return _minimum; }
        }

        /// <summary>
        /// Gets the maximum inclusive value of this range.
        /// </summary>
        /// <value>
        /// The maximum inclusive value of this range.
        /// </value>
        /// <seealso cref="Minimum"/>
        public T Maximum
        {
            get { return _maximum; }
        }

        private readonly T _minimum, _maximum;
    }
}
