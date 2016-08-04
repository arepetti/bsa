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
using System.Linq;
using System.Numerics;

namespace Bsa.Dsp.Filters.Iir
{
    sealed class ChebyshevIOnlineFilterDesigner : FisherMethodFilterDesigner
    {
        protected internal override IOnlineFilter CreateLowPass(FilterDesignSettings settings, double frequency)
        {
            return Create(Design(FilterKind.LowPass, settings.Order,
                CalculateRipple(settings),
                settings.NormalizeFrequency(frequency)));
        }

        protected internal override IOnlineFilter CreateHighPass(FilterDesignSettings settings, double frequency)
        {
            return Create(Design(FilterKind.HighPass, settings.Order,
                CalculateRipple(settings),
                settings.NormalizeFrequency(frequency)));
        }

        protected internal override IOnlineFilter CreateNotch(FilterDesignSettings settings, double frequency)
        {
            const double notchHalfWidth = 0.5; // Hz

            return Create(Design(FilterKind.BandStop, settings.Order,
                CalculateRipple(settings),
                settings.NormalizeFrequency(frequency - notchHalfWidth),
                settings.NormalizeFrequency(frequency + notchHalfWidth)));
        }

        protected internal override IOnlineFilter CreateBandPass(FilterDesignSettings settings, double lowFrequency, double highFrequency)
        {
            return Create(Design(FilterKind.BandPass, settings.Order,
                CalculateRipple(settings),
                settings.NormalizeFrequency(lowFrequency),
                settings.NormalizeFrequency(highFrequency)));
        }

        protected internal override IOnlineFilter CreateBandStop(FilterDesignSettings settings, double lowFrequency, double highFrequency)
        {
            return Create(Design(FilterKind.BandStop, settings.Order,
                CalculateRipple(settings),
                settings.NormalizeFrequency(lowFrequency),
                settings.NormalizeFrequency(highFrequency)));
        }

        protected override Complex[] GetPoles(int filterOrder, double ripple)
        {
            var poles = Enumerable.Range(0, filterOrder)
                .Select(i => Mathx.Expj((filterOrder / 2.0 + 0.5 + i) * Math.PI / filterOrder))
                .ToArray();

            double rip = Math.Pow(10, -ripple / 10);
            double eps = Math.Sqrt(rip - 1);
            double y = Mathx.Asinh(1.0 / eps) / filterOrder;

            double sinhY = Math.Sinh(y), coshY = Math.Cosh(y);
            for (int i = 0; i < filterOrder; ++i)
                poles[i] = new Complex(poles[i].Real * sinhY, poles[i].Imaginary * coshY);

            return poles;
        }

        private static double CalculateRipple(FilterDesignSettings settings)
        {
            return settings.GetOrDefault<ChebyshevFilterDesignSettings, double>(x => x.MaximumRipple ?? 0, 0);
        }
    }
}
