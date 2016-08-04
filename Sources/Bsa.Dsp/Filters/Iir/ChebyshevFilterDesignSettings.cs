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

namespace Bsa.Dsp.Filters.Iir
{
    /// <summary>
    /// Contains settings to design a Chebyshev filter.
    /// </summary>
    public sealed class ChebyshevFilterDesignSettings : FilterDesignSettings
    {
        /// <summary>
        /// Gets/sets the maximum ripple (in dB) for this filter.
        /// </summary>
        /// <value>
        /// The maximum ripple (in dB) for this filter or <see langword="null"/> if this value is unspecified
        /// and default/optimal should be used.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If value is greater than zero or <see cref="Double.NaN"/> or <see cref="Double.PositiveInfinity"/>.
        /// </exception>
        public double? MaximumRipple
        {
            get { return _maximumRipple; }
            set
            {
                if (value.HasValue)
                {
                    if (value.Value > 0 || Double.IsNaN(value.Value) || Double.IsPositiveInfinity(value.Value))
                        throw new ArgumentOutOfRangeException();
                }

                _maximumRipple = value;
            }
        }

        private double? _maximumRipple;
    }
}
