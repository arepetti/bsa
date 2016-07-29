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
    public static class OnlineFilterFactory
    {
        public static IOnlineFilter Create(FilterType type, OnlineFilterDesigner design, FilterDesignSettings settings)
        {
            if (type != FilterType.AllPass)
                throw new ArgumentException("This overload can be used only to create all pass filters.", "type");

            ValidateSettingsWithDesigner(design, settings);

            return design.CreateAllPass(settings);
        }

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

        public static IOnlineFilter Create(FilterType type, OnlineFilterDesigner design, FilterDesignSettings settings, Range<double> band)
        {
            ValidateSettingsWithDesigner(design, settings);
            ValidateFrequency(band.Minimum);
            ValidateFrequency(band.Maximum);

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

            if (settings.Order <= 0)
                throw new ArgumentOutOfRangeException("Filter order cannot be zero or negative", "settings.Order");

            if (settings.SamplingRate <= 0)
                throw new ArgumentOutOfRangeException("Sampling rate cannot be zero or negative.", "settings.SamplingRate");
        }

        private static void ValidateFrequency(double frequency)
        {
            if (Double.IsNaN(frequency) || Double.IsInfinity(frequency))
                throw new ArgumentOutOfRangeException("Cutoff frequency cannot be NaN or Infinity.", "frequency");
        }
    }
}
