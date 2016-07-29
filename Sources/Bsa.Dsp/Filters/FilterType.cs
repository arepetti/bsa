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
    /// Represents the type of a filter.
    /// </summary>
    /// <remarks>
    /// Not all filter designers support all filter types, see documentation of each designer
    /// for a detailed list of supported filter types.
    /// </remarks>
    public enum FilterType
    {
        /// <summary>
        /// All-pass filter: passes all frequencies equally in gain, but changes the phase
        /// relationship among various frequencies.
        /// </summary>
        AllPass,

        /// <summary>
        /// Low-pass filter: passes signals with a frequency lower than a certain cutoff frequency
        /// and attenuates signals with frequencies higher than the cutoff frequency.
        /// </summary>
        LowPass,

        /// <summary>
        /// High-pass filter: passes signals with a frequency higher than a certain cutoff frequency
        /// and attenuates signals with frequencies lower than the cutoff frequency
        /// </summary>
        HighPass,

        /// <summary>
        /// Band-stop filter: passes frequencies outside a certain range and rejects (attenuates) frequencies
        /// within that range.
        /// </summary>
        BandStop,

        /// <summary>
        /// Band-pass filter: passes frequencies within a certain range and rejects (attenuates) frequencies
        /// outside that range.
        /// </summary>
        BandPass,

        /// <summary>
        /// Low-shelf filter: passes all frequencies, but increases or reduces frequencies below the shelf
        /// frequency by specified amount.
        /// </summary>
        LowShelf,

        /// <summary>
        /// High-shelf fitler: passes all frequencies, but increases or reduces frequencies above the shelf
        /// frequency by specified amount.
        /// </summary>
        HighShelf,

        /// <summary>
        /// Notch filter: stops a very narrow range of frequencies (a band-stop filter with a very high Q).
        /// </summary>
        Notch,

        /// <summary>
        /// Peak filter: makes a peak or a dip in the frequency response.
        /// </summary>
        Peak
    }
}
