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
    /// Contains settings to design a Robert Bristow-Johnson filters.
    /// </summary>
    public sealed class RbjFilterDesignSettings : FilterDesignSettings
    {
        /// <summary>
        /// Gets/sets the quality factor of this filter.
        /// </summary>
        /// <value>
        /// The quality factor of this fitler, default value is 1.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If value is zero or less,  <see cref="Double.NaN"/> or greater than one.
        /// </exception>
        public double Quality
        {
            get { return _quality; }
            set
            {
                if (value <= 0 || value > 1 || Double.IsNaN(value))
                    throw new ArgumentOutOfRangeException();

                _quality = value;
            }
        }

        /// <summary>
        /// Gets/sets the gain (in dB).
        /// </summary>
        /// <value>
        /// The gain (expressed in dB), it is used only for <see cref="FilterKind.Peak"/>, <see cref="FilterKind.HighShelf"/>
        /// and <see cref="FilterKind.LowShelf"/>. Default value is 0.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If value is <see cref="Double.NaN"/> or infinity.
        /// </exception>
        public double Gain
        {
            get { return _gain; }
            set
            {
                if (Double.IsInfinity(value) || Double.IsNaN(value))
                    throw new ArgumentOutOfRangeException();

                _gain = value;
            }
        }

        internal const double DefaultQuality = 1;
        internal const double DefaultGain = 0;

        private double _quality = DefaultQuality;
        private double _gain = DefaultGain;
    }
}
