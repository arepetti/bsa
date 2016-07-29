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
using System.ComponentModel;

namespace Bsa.Dsp.Filters
{
    /// <summary>
    /// Factory class for online filters.
    /// </summary>
    public static class OnlineFilterFactory
    {
        /// <overload>
        /// Creates a new filter.
        /// </overload>
        /// <summary>
        /// Creates a new all-pass filter.
        /// </summary>
        /// <param name="type">Type of the filter, it must be <see cref="FilterType.AllPass"/>.</param>
        /// <param name="design">Designer used to build the filter (it must support creation of an all-pass filter).</param>
        /// <param name="settings">
        /// Settings for the filter (base class <see cref="FilterDesignSettings"/> may be used if you do not want to specify
        /// any of extra settings supported by designer for this specific type of filter).
        /// </param>
        /// <returns>
        /// An on-line filter with specified type and characteristics.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="design"/> is <see langword="null"/>.
        /// <br/>-or-<br/>
        /// If <paramref name="settings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="type"/> is not <see cref="FilterType.AllPass"/>.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If specified <paramref name="design"/> does not support required filter <paramref name="type"/>.
        /// </exception>
        public static IOnlineFilter Create(FilterType type, OnlineFilterDesigner design, FilterDesignSettings settings)
        {
            if (type != FilterType.AllPass)
                throw new ArgumentException("This overload can be used only to create all pass filters.", "type");

            ValidateSettingsWithDesigner(design, settings);

            return design.CreateAllPass(settings);
        }

        /// <summary>
        /// Creates a new filter with specified cutoff frequency.
        /// </summary>
        /// <param name="type">
        /// Type of the filter, it must be a filter with a single cutoff frequency (not <see cref="FilterType.AllPass"/>
        /// or another band filter).
        /// </param>
        /// <param name="design">Designer used to build the filter (it must support creation of required filter).</param>
        /// <param name="settings">
        /// Settings for the filter (base class <see cref="FilterDesignSettings"/> may be used if you do not want to specify
        /// any of extra settings supported by designer for this specific type of filter).
        /// </param>
        /// <param name="frequency">Cutoff frequency for this filter.</param>
        /// <returns>
        /// An on-line filter with specified type and characteristics.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="design"/> is <see langword="null"/>.
        /// <br/>-or-<br/>
        /// If <paramref name="settings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="frequency"/> is 0 or less or <see cref="Double.NaN"/> or <see cref="Double.PositiveInfinity"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="type"/> is not a filter with a single cutoff frequency (<see cref="FilterType.AllPass"/>,
        /// <see cref="FilterType.BandPass"/> and <see cref="FilterType.BandStop"/> cannot be created with this function).
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If specified <paramref name="design"/> does not support required filter <paramref name="type"/>.
        /// </exception>
        public static IOnlineFilter Create(FilterType type, OnlineFilterDesigner design, FilterDesignSettings settings, double frequency)
        {
            ValidateSettingsWithDesigner(design, settings);
            ValidateFrequency(frequency);

            switch (type)
            {
                case FilterType.BandPass:
                case FilterType.BandStop:
                case FilterType.AllPass:
                    throw new ArgumentException("This overload can be used only to create filters with a cutoff frequency.", "type");
                case FilterType.LowPass:
                    return design.CreateLowPass(settings, frequency);
                case FilterType.HighPass:
                    return design.CreateHighPass(settings, frequency);
                case FilterType.LowShelf:
                    return design.CreateLowShelf(settings, frequency);
                case FilterType.HighShelf:
                    return design.CreateHighShelf(settings, frequency);
                case FilterType.Notch:
                    return design.CreateNotch(settings, frequency);
                case FilterType.Peak:
                    return design.CreatePeak(settings, frequency);
                default:
                    throw new InvalidEnumArgumentException("type", (int)type, typeof(FilterType));
            }
        }

        /// <summary>
        /// Creates a new band filter.
        /// </summary>
        /// <param name="type">
        /// Type of the filter, it must be a band filter with a single cutoff frequency (not <see cref="FilterType.AllPass"/>
        /// or another single cutoff frequency filter).
        /// </param>
        /// <param name="design">Designer used to build the filter (it must support creation of required filter).</param>
        /// <param name="settings">
        /// Settings for the filter (base class <see cref="FilterDesignSettings"/> may be used if you do not want to specify
        /// any of extra settings supported by designer for this specific type of filter).
        /// </param>
        /// <param name="band">Frequency band of this filter.</param>
        /// <returns>
        /// An on-line filter with specified type and characteristics.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="design"/> is <see langword="null"/>.
        /// <br/>-or-<br/>
        /// If <paramref name="settings"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If one of the frequencies is 0 or less or <see cref="Double.NaN"/> or <see cref="Double.PositiveInfinity"/>.
        /// <br/>-or-<br/>
        /// If <c>band.Minimum</c> is greater than <c>band.Maximum</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="type"/> is not a band filter.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If specified <paramref name="design"/> does not support required filter <paramref name="type"/>.
        /// </exception>
        public static IOnlineFilter Create(FilterType type, OnlineFilterDesigner design, FilterDesignSettings settings, Range<double> band)
        {
            ValidateSettingsWithDesigner(design, settings);
            ValidateFrequency(band.Minimum);
            ValidateFrequency(band.Maximum);

            if (band.Maximum < band.Minimum)
                throw new ArgumentOutOfRangeException("band");

            if (type == FilterType.BandPass)
                return design.CreateBandPass(settings, band.Minimum, band.Maximum);
            else if (type == FilterType.BandStop)
                return design.CreateBandPass(settings, band.Minimum, band.Maximum);

            throw new ArgumentException("This overload can be used only to create band filters.", "type");
        }

        private static void ValidateSettingsWithDesigner(OnlineFilterDesigner design, FilterDesignSettings settings)
        {
            if (design == null)
                throw new ArgumentNullException("design");

            if (settings == null)
                throw new ArgumentNullException("settings");

            // Add here more (generic) validation when required...
        }

        private static void ValidateFrequency(double frequency)
        {
            if (Double.IsNaN(frequency) || Double.IsInfinity(frequency))
                throw new ArgumentOutOfRangeException("Cutoff frequency cannot be NaN or Infinity.", "frequency");

            // TODO: should also check frequency vs sampling rate?
        }
    }
}
