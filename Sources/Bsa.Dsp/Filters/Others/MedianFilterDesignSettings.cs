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

namespace Bsa.Dsp.Filters.Others
{
    /// <summary>
    /// Contains settings specific for <see cref="OnlineFilterDesign.Median"/>.
    /// </summary>
    public sealed class MedianFilterDesignSettings : FilterDesignSettings
    {
        /// <summary>
        /// Gets/sets the window size of the median filter.
        /// </summary>
        /// <value>
        /// The window size of the median filter, default value is 2.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If value is less or equal than zero.
        /// </exception>
        public int WindowSize
        {
            get { return _windowSize; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();

                _windowSize = value;
            }
        }

        /// <devdoc>
        /// Default value for DefaultWindowSize property, it's used by filter designer
        /// when invoked with base class FilterDesignSettings as argument.
        /// </devdoc>
        internal const int DefaultWindowSize = 2;

        private int _windowSize = DefaultWindowSize;
    }
}
