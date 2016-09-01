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
using System.Diagnostics;
using System.Linq;

namespace Bsa.Dsp.Filters.Iir
{
    /// <devdoc>
    /// Robert Bristow-Johnson, http://www.musicdsp.org/files/Audio-EQ-Cookbook.txt
    /// </devdoc>
    sealed class RbjFilterDesigner : OnlineFilterDesigner
    {
        protected internal override IOnlineFilter CreateLowPass(FilterDesignSettings settings, double frequency)
        {
            Debug.Assert(settings != null);
            Debug.Assert(settings.SamplingRate > 0);

            double omega = (2 * Math.PI) * (frequency / settings.SamplingRate);
            double tsin = Math.Sin(omega);
            double tcos = Math.Cos(omega);
            double alpha = tsin / (2 * Resolve(settings, x => x.Quality, RbjFilterDesignSettings.DefaultQuality));

            double b0 = (1 - tcos) / 2;
            double b1 = 1 - tcos;
            double b2 = (1 - tcos) / 2;
            double a0 = 1 + alpha;
            double a1 = -2 * tcos;
            double a2 = 1 - alpha;

            return CreateCascade(settings.Order, new IirFilterCoefficients { B = new double[] { b0, b1, b2 }, A = new double[] { a0, a1, a2 } });
        }

        protected internal override IOnlineFilter CreateHighPass(FilterDesignSettings settings, double frequency)
        {
            Debug.Assert(settings != null);
            Debug.Assert(settings.SamplingRate > 0);

            double omega = (2 * Math.PI) * (frequency / settings.SamplingRate);
            double tsin = Math.Sin(omega);
            double tcos = Math.Cos(omega);
            double alpha = tsin / (2 * Resolve(settings, x => x.Quality, RbjFilterDesignSettings.DefaultQuality));

            double b0 = (1 + tcos) / 2;
            double b1 = -(1 + tcos);
            double b2 = (1 + tcos) / 2;
            double a0 = 1 + alpha;
            double a1 = -2 * tcos;
            double a2 = 1 - alpha;

            return CreateCascade(settings.Order, new IirFilterCoefficients { B = new double[] { b0, b1, b2 }, A = new double[] { a0, a1, a2 } });
        }

        protected internal override IOnlineFilter CreateNotch(FilterDesignSettings settings, double frequency)
        {
            Debug.Assert(settings != null);
            Debug.Assert(settings.SamplingRate > 0);

            double omega = (2 * Math.PI) * (frequency / settings.SamplingRate);
            double tsin = Math.Sin(omega);
            double tcos = Math.Cos(omega);
            double alpha = tsin / (2 * Resolve(settings, x => x.Quality, RbjFilterDesignSettings.DefaultQuality));

            double b0 = 1;
            double b1 = -2 * tcos;
            double b2 = 1;
            double a0 = 1 + alpha;
            double a1 = -2 * tcos;
            double a2 = 1 - alpha;

            return CreateCascade(settings.Order, new IirFilterCoefficients { B = new double[] { b0, b1, b2 }, A = new double[] { a0, a1, a2 } });
        }

        protected internal override IOnlineFilter CreatePeak(FilterDesignSettings settings, double frequency)
        {
            Debug.Assert(settings != null);
            Debug.Assert(settings.SamplingRate > 0);

            double omega = (2 * Math.PI) * (frequency / settings.SamplingRate);
            double tsin = Math.Sin(omega);
            double tcos = Math.Cos(omega);
            double alpha = tsin / (2 * Resolve(settings, x => x.Quality, RbjFilterDesignSettings.DefaultQuality));
            double a = Math.Pow(10, Resolve(settings, x => x.Gain, RbjFilterDesignSettings.DefaultGain) / 40);
            double w0 = Math.PI * 2 * frequency / settings.SamplingRate;

            double b0 = 1 + alpha * a;
            double b1 = -2 * Math.Cos(w0);
            double b2 = 1 - alpha * a;
            double a0 = 1 + alpha / a;
            double a1 = 2 * Math.Cos(w0);
            double a2 =  1 - alpha / a;

            return CreateCascade(settings.Order, new IirFilterCoefficients { B = new double[] { b0, b1, b2 }, A = new double[] { a0, a1, a2 } });
        }

        /// <devdoc>
        /// Implementation with constant 0 dB peak gain.
        /// </devdoc>
        protected internal override IOnlineFilter CreateBandPass(FilterDesignSettings settings, double lowFrequency, double highFrequency)
        {
            Debug.Assert(settings != null);
            Debug.Assert(settings.SamplingRate > 0);

            double frequency = CalculateCenterFrequency(lowFrequency, highFrequency);
            double q = CalculateQ(lowFrequency, highFrequency);
            double omega = (2 * Math.PI) * (frequency / settings.SamplingRate);
            double tsin = Math.Sin(omega);
            double tcos = Math.Cos(omega);
            double alpha = tsin / (2 * q);
            double w0 = Math.PI * 2 * frequency / settings.SamplingRate;

            double b0 = alpha;
            double b1 =  0;
            double b2 = -alpha;
            double a0 = 1 + alpha;
            double a1 = -2 * Math.Cos(w0);
            double a2 = 1 - alpha;

            return CreateCascade(settings.Order, new IirFilterCoefficients { B = new double[] { b0, b1, b2 }, A = new double[] { a0, a1, a2 } });
        }

        protected internal override IOnlineFilter CreateAllPass(FilterDesignSettings settings, double lowFrequency, double highFrequency)
        {
            Debug.Assert(settings != null);
            Debug.Assert(settings.SamplingRate > 0);

            double frequency = CalculateCenterFrequency(lowFrequency, highFrequency);
            double q = CalculateQ(lowFrequency, highFrequency);
            double omega = (2 * Math.PI) * (frequency / settings.SamplingRate);
            double tsin = Math.Sin(omega);
            double tcos = Math.Cos(omega);
            double alpha = tsin / (2 * q);
            double w0 = Math.PI * 2 * frequency / settings.SamplingRate;

            double b0 = 1 - alpha;
            double b1 = -2 * Math.Cos(w0);
            double b2 = 1 + alpha;
            double a0 = 1 + alpha;
            double a1 = -2 * Math.Cos(w0);
            double a2 = 1 - alpha;

            return CreateCascade(settings.Order, new IirFilterCoefficients { B = new double[] { b0, b1, b2 }, A = new double[] { a0, a1, a2 } });
        }

        protected internal override IOnlineFilter CreateBandStop(FilterDesignSettings settings, double lowFrequency, double highFrequency)
        {
            Debug.Assert(settings != null);
            Debug.Assert(settings.SamplingRate > 0);

            double frequency = CalculateCenterFrequency(lowFrequency, highFrequency);
            double q = CalculateQ(lowFrequency, highFrequency);
            double omega = (2 * Math.PI) * (frequency / settings.SamplingRate);
            double tsin = Math.Sin(omega);
            double tcos = Math.Cos(omega);
            double alpha = tsin / (2 * q);

            double b0 = 1;
            double b1 = -2 * tcos;
            double b2 = 1;
            double a0 = 1 + alpha;
            double a1 = -2 * tcos;
            double a2 = 1 - alpha;

            return CreateCascade(settings.Order, new IirFilterCoefficients { B = new double[] { b0, b1, b2 }, A = new double[] { a0, a1, a2 } });
        }

        protected internal override IOnlineFilter CreateLowShelf(FilterDesignSettings settings, double frequency)
        {
            Debug.Assert(settings != null);
            Debug.Assert(settings.SamplingRate > 0);

            double omega = (2 * Math.PI) * (frequency / settings.SamplingRate);
            double tsin = Math.Sin(omega);
            double tcos = Math.Cos(omega);
            double alpha = tsin / (2 * Resolve(settings, x => x.Quality, RbjFilterDesignSettings.DefaultQuality));
            double a = Math.Pow(10, Resolve(settings, x => x.Gain, RbjFilterDesignSettings.DefaultGain) / 40);
            double w0 = Math.PI * 2 * frequency / settings.SamplingRate;

            double b0 = a * ((a + 1) - (a - 1) * Math.Cos(w0) + 2 * Math.Sqrt(a) * alpha);
            double b1 = 2 * a * ((a - 1) - (a + 1) * Math.Cos(w0));
            double b2 = a * ((a + 1) - (a - 1) * Math.Cos(w0) - 2 * Math.Sqrt(a) * alpha);
            double a0 = (a + 1) + (a - 1) * Math.Cos(w0) + 2 * Math.Sqrt(a) * alpha;
            double a1 = -2 * ((a - 1) + (a + 1) * Math.Cos(w0));
            double a2 = (a + 1) + (a - 1) * Math.Cos(w0) - 2 * Math.Sqrt(a) * alpha;

            return CreateCascade(settings.Order, new IirFilterCoefficients { B = new double[] { b0, b1, b2 }, A = new double[] { a0, a1, a2 } });
        }

        protected internal override IOnlineFilter CreateHighShelf(FilterDesignSettings settings, double frequency)
        {
            Debug.Assert(settings != null);
            Debug.Assert(settings.SamplingRate > 0);

            double omega = (2 * Math.PI) * (frequency / settings.SamplingRate);
            double tsin = Math.Sin(omega);
            double tcos = Math.Cos(omega);
            double alpha = tsin / (2 * Resolve(settings, x => x.Quality, RbjFilterDesignSettings.DefaultQuality));
            double a = Math.Pow(10, Resolve(settings, x => x.Gain, RbjFilterDesignSettings.DefaultGain) / 40);
            double w0 = Math.PI * 2 * frequency / settings.SamplingRate;

            double b0 = a * ((a + 1) + (a - 1) * Math.Cos(w0) + 2 * Math.Sqrt(a) * alpha);
            double b1 = -2 * a * ((a - 1) + (a + 1) * Math.Cos(w0));
            double b2 = a * ((a + 1) + (a - 1) * Math.Cos(w0) - 2 * Math.Sqrt(a) * alpha);
            double a0 = (a + 1) - (a - 1) * Math.Cos(w0) + 2 * Math.Sqrt(a) * alpha;
            double a1 = 2 * ((a - 1) - (a + 1) * Math.Cos(w0));
            double a2 = (a + 1) - (a - 1) * Math.Cos(w0) - 2 * Math.Sqrt(a) * alpha;

            return CreateCascade(settings.Order, new IirFilterCoefficients { B = new double[] { b0, b1, b2 }, A = new double[] { a0, a1, a2 } });
        }

        private static T Resolve<T>(FilterDesignSettings settings, Func<RbjFilterDesignSettings, T> selector, T defaultValue)
        {
            Debug.Assert(selector != null);

            var rbjSettings = settings as RbjFilterDesignSettings;
            if (rbjSettings != null)
                return selector(rbjSettings);
            
            return defaultValue;
        }

        private static double CalculateCenterFrequency(double low, double high)
        {
            return Math.Sqrt(low * high);
        }

        /// <devdoc>
        /// Dennis Bohn, http://www.rane.com/pdf/ranenotes/Bandwidth_in_Octaves_vs_Q_in_Bandpass_Filters.pdf.
        /// Note that this is different from Robert's proposed implementation <c>1/Q = 2*sinh(ln(2)/2*BW*w0/sin(w0))</c>.
        /// </devdoc>
        private static double CalculateBandwidth(double low, double high)
        {
            double q = CalculateQ(low, high), q2 = Mathx.Sqr(q);
            return (Math.Log10(1 + 1 / (2 * q2)) + Math.Sqrt((Mathx.Sqr(2 + (1 / q2)) / 4) - 1)) / Math.Log10(2);
        }

        private static double CalculateQ(double low, double high)
        {
            return CalculateCenterFrequency(low, high) / (high - low);
        }

        private static IOnlineFilter CreateCascade(int order, IirFilterCoefficients coefficients)
        {
            return new FilterCascade(Enumerable.Range(0, order).Select(_ => new BiquadFilter(coefficients)).ToArray());
        }
    }
}
