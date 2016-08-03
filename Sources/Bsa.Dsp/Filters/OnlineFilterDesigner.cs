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

namespace Bsa.Dsp.Filters
{
    /// <summary>
    /// Base class for filter designers.
    /// </summary>
    /// <remarks>
    /// Each derived class (a specific designer) will implement one or more of factory methods exposed by
    /// this base class. Unimplemented methods (when a default implementation is not available) will throw
    /// <see cref="NotSupportedException"/>.
    /// </remarks>
    public abstract class OnlineFilterDesigner
    {
        /// <summary>
        /// Creates a designer specific filter.
        /// </summary>
        /// <param name="settings">Filter settings.</param>
        /// <returns>The required filter.</returns>
        /// <exception cref="NotSupportedException">
        /// This designer cannot create the required filter layout.
        /// </exception>
        protected internal virtual IOnlineFilter CreateOther(FilterDesignSettings settings)
        {
            return NotSupported();
        }

        /// <summary>
        /// Creates an all-pass filter.
        /// </summary>
        /// <param name="settings">Filter settings.</param>
        /// <returns>The required filter.</returns>
        /// <exception cref="NotSupportedException">
        /// This designer cannot create the required filter layout.
        /// </exception>
        protected internal virtual IOnlineFilter CreateAllPass(FilterDesignSettings settings)
        {
            return NotSupported();
        }

        /// <summary>
        /// Creates a low-pass filter.
        /// </summary>
        /// <param name="settings">Filter settings.</param>
        /// <param name="frequency">Cutoff frequency.</param>
        /// <returns>The required filter.</returns>
        /// <exception cref="NotSupportedException">
        /// This designer cannot create the required filter layout.
        /// </exception>
        protected internal virtual IOnlineFilter CreateLowPass(FilterDesignSettings settings, double frequency)
        {
            return NotSupported();
        }

        /// <summary>
        /// Creates a high-pass filter.
        /// </summary>
        /// <param name="settings">Filter settings.</param>
        /// <param name="frequency">Cutoff frequency.</param>
        /// <returns>The required filter.</returns>
        /// <exception cref="NotSupportedException">
        /// This designer cannot create the required filter layout.
        /// </exception>
        protected internal virtual IOnlineFilter CreateHighPass(FilterDesignSettings settings, double frequency)
        {
            return NotSupported();
        }

        /// <summary>
        /// Creates a band-stop filter.
        /// </summary>
        /// <param name="settings">Filter settings.</param>
        /// <param name="lowFrequency">Low cutoff frequency.</param>
        /// <param name="highFrequency">High cutoff frequency.</param>
        /// <returns>The required filter.</returns>
        /// <exception cref="NotSupportedException">
        /// This designer cannot create the required filter layout.
        /// </exception>
        protected internal virtual IOnlineFilter CreateBandStop(FilterDesignSettings settings, double lowFrequency, double highFrequency)
        {
            return NotSupported();
        }

        /// <summary>
        /// Creates a band-pass filter.
        /// </summary>
        /// <param name="settings">Filter settings.</param>
        /// <param name="lowFrequency">Low cutoff frequency.</param>
        /// <param name="highFrequency">High cutoff frequency.</param>
        /// <returns>The required filter.</returns>
        /// <exception cref="NotSupportedException">
        /// This designer cannot create the required filter layout.
        /// </exception>
        protected internal virtual IOnlineFilter CreateBandPass(FilterDesignSettings settings, double lowFrequency, double highFrequency)
        {
            // Default implementation if there is not a better designer-specific version, if one of required
            // filters (for example one designer implements low-pass but not high-pass) isn't supported then it will
            // still throw a NotSupportedException for the missing layout.
            return new FilterCascade(
                CreateHighPass(settings, lowFrequency),
                CreateLowPass(settings, highFrequency));
        }

        /// <summary>
        /// Creates a low-shelf filter.
        /// </summary>
        /// <param name="settings">Filter settings.</param>
        /// <param name="frequency">Cutoff frequency.</param>
        /// <returns>The required filter.</returns>
        /// <exception cref="NotSupportedException">
        /// This designer cannot create the required filter layout.
        /// </exception>
        protected internal virtual IOnlineFilter CreateLowShelf(FilterDesignSettings settings, double frequency)
        {
            return NotSupported();
        }

        /// <summary>
        /// Creates a high-shelf filter.
        /// </summary>
        /// <param name="settings">Filter settings.</param>
        /// <param name="frequency">Cutoff frequency.</param>
        /// <returns>The required filter.</returns>
        /// <exception cref="NotSupportedException">
        /// This designer cannot create the required filter layout.
        /// </exception>
        protected internal virtual IOnlineFilter CreateHighShelf(FilterDesignSettings settings, double frequency)
        {
            return NotSupported();
        }

        /// <summary>
        /// Creates a notch filter.
        /// </summary>
        /// <param name="settings">Filter settings.</param>
        /// <param name="frequency">Notch frequency.</param>
        /// <returns>The required filter.</returns>
        /// <exception cref="NotSupportedException">
        /// This designer cannot create the required filter layout.
        /// </exception>
        protected internal virtual IOnlineFilter CreateNotch(FilterDesignSettings settings, double frequency)
        {
            return NotSupported();
        }

        /// <summary>
        /// Creates a peak filter.
        /// </summary>
        /// <param name="settings">Filter settings.</param>
        /// <param name="frequency">Peak frequency.</param>
        /// <returns>The required filter.</returns>
        /// <exception cref="NotSupportedException">
        /// This designer cannot create the required filter layout.
        /// </exception>
        protected internal virtual IOnlineFilter CreatePeak(FilterDesignSettings settings, double frequency)
        {
            return NotSupported();
        }

        private static IOnlineFilter NotSupported()
        {
            throw new NotSupportedException("This filter type is not supported by this filter designer.");
        }
    }
}
