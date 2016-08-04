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
    /// Base class for all the filter design settings.
    /// </summary>
    public abstract class FilterDesignSettingsBase
    {
        /// <summary>
        /// Gets/sets the sampling rate of data to filter.
        /// </summary>
        /// <value>
        /// The sampling rate (in Hz) of data to filter. Default value is 0 and it's an invalid input
        /// for most filters (it has to be assigned before use).
        /// </value>
        /// <remarks>
        /// Even if sampling <em>rate</em> is technically the incorrect word for this value I found that it
        /// is in such common use that moving to sampling <em>frequency</em> or sampling <em>period</em>
        /// would be even more confusing. Don't forget that (unless we introduce a <c>Quantity</c> object
        /// which embeds its unit of measure) this value is always expressed in Hertz.
        /// </remarks>
        public double SamplingRate
        {
            get { return _samplingRate; }
            set
            {
                if (value <= 0 || Double.IsNaN(value) || Double.IsPositiveInfinity(value))
                    throw new ArgumentOutOfRangeException();

                _samplingRate = value;
            }
        }

        internal TResult GetOrDefault<TDerivedClass, TResult>(Func<TDerivedClass, TResult> selector, TResult defaultValue)
            where TDerivedClass : FilterDesignSettingsBase
        {
            var derivedClass = this as TDerivedClass;
            if (derivedClass != null)
                return selector(derivedClass);

            return defaultValue;
        }

        internal double NormalizeFrequency(double frequency)
        {
            return Mathx.Clip(0, 0.5, SamplingRate / frequency);
        }

        private double _samplingRate;
    }
}
