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

namespace Bsa.Dsp
{
    /// <summary>
    /// Represents a null-filter which does nothing on its input.
    /// </summary>
    public sealed class NullFilter : IOnlineFilter
    {
        /// <summary>
        /// Gets a shared, thread-safe, singleton instance of this filter.
        /// </summary>
        public static readonly IOnlineFilter Instance = new NullFilter();

        /// <summary>
        /// Returns the input sample, unchanged.
        /// </summary>
        /// <param name="sample">Sample to filter.</param>
        /// <returns>The input sample, unchanged.</returns>
        public double Process(double sample)
        {
            return sample;
        }

        /// <summary>
        /// Indicates whether this filter is enabled.
        /// </summary>
        /// <value>
        /// Always <see langword="true"/> because this is a shared stateless instance filter
        /// but its behavior is exactly the one of a disabled filter (output copies input).
        /// </value>
        bool IOnlineProcessor.IsEnabled
        {
            get { return true; }
            set { }
        }

        /// <summary>
        /// Does nothing, this filter has no state.
        /// </summary>
        void IOnlineProcessor.Reset()
        {
        }

        /// <summary>
        /// Does nothing, this filter does not acquire any resource.
        /// </summary>
        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
