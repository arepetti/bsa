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
    public abstract class OnlineFilterDesigner
    {
        public virtual IOnlineFilter CreateAllPass(FilterDesignSettings settings)
        {
            return ThrowNotSupported();
        }

        public virtual IOnlineFilter CreateLowPass(FilterDesignSettings settings, double frequency)
        {
            return ThrowNotSupported();
        }

        public virtual IOnlineFilter CreateHighPass(FilterDesignSettings settings, double frequency)
        {
            return ThrowNotSupported();
        }

        public virtual IOnlineFilter CreateBandStop(FilterDesignSettings settings, double lowFrequency, double highFrequency)
        {
            return ThrowNotSupported();
        }

        public virtual IOnlineFilter CreateBandPass(FilterDesignSettings settings, double lowFrequency, double highFrequency)
        {
            return ThrowNotSupported();
        }

        public virtual IOnlineFilter CreateLowShelf(FilterDesignSettings settings, double frequency)
        {
            return ThrowNotSupported();
        }

        public virtual IOnlineFilter CreateHighShelf(FilterDesignSettings settings, double frequency)
        {
            return ThrowNotSupported();
        }

        public virtual IOnlineFilter CreateNotch(FilterDesignSettings settings, double frequency)
        {
            return ThrowNotSupported();
        }

        public virtual IOnlineFilter CreatePeak(FilterDesignSettings settings, double frequency)
        {
            return ThrowNotSupported();
        }

        private static IOnlineFilter ThrowNotSupported()
        {
            throw new NotSupportedException("This filter type is not supported by this filter designer.");
        }
    }
}
