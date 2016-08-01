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

namespace Bsa.Dsp.Filters
{
    /// <summary>
    /// Contains settings for most filter designers.
    /// </summary>
    public class FilterDesignSettings : FilterDesignSettingsBase
    {
        /// <summary>
        /// Gets/sets the order (or length) of the filter.
        /// </summary>
        /// <value>
        /// The order (or length) of the filter. Default value is 0 and may not be valid for some
        /// filter designers (they may throw an exception while trying to design the filter).
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If value is less than zero.
        /// </exception>
        public int Order
        {
            get { return _order; }
            set
            {
                // Order 0 means that order is not applicable for this kind of filter,
                // each designer will throw an exception if this value is not appropriate
                if (value < 0)
                    throw new ArgumentOutOfRangeException();

                _order = value;
            }
        }

        private int _order;
    }
}
