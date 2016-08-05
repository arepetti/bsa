//
// Original algorithm and code from T. Fisher (http://www-users.cs.york.ac.uk/~fisher/),
// see also Fisher: "Digital signal processing of Decca Navigator radionavigation signals" (1999).
//
//
// This C# code is an adaption of Java port from original C code by Christian d'Heureuse (2013).
// Java version is multi-licensed under EPL 1.0 (or later) and L-GPL 2.1 (or later).
// More details at http://svn.source-code.biz/dsp-java/trunk/src/main/java/biz/source_code/dsp/filter/IirFilterDesignFisher.java
//
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Bsa.Dsp.Filters.Iir
{
    abstract class FisherMethodFilterDesigner : OnlineFilterDesigner
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
        
        /// <devdoc>
        /// http://en.wikipedia.org/wiki/Prototype_filter.
        /// </devdoc>
        protected abstract Complex[] GetPoles(int filterOrder, double ripple);

        protected virtual double CalculateRipple(FilterDesignSettings settings)
        {
            // Default to 0 because most filters don't use Ripple
            return 0;
        }

        protected virtual SToZMappingMethod GetSToZMappingMethod()
        {
            return SToZMappingMethod.BilinearTransform; // MatchedZTransform is for Bessel filters
        }

        /// <devdoc>
        /// Designs an IIR filter and returns the IIR filter coefficients. Cutoff frequencies
        /// are relative to sampling rate and must be between 0 and 0.5.
        /// Passband ripple is in dB. Must be negative. Only used for Chebyshev filter, ignored for other filters.
        /// Both low-pass and high-pass use fcf1, band-pass and band-stop need both.
        /// </devdoc>
        private IirFilterCoefficients Design(FilterKind type, int filterOrder, double ripple, double fcf1, double fcf2 = 0)
        {
            var poles = GetPoles(filterOrder, ripple);

            SToZMappingMethod sToZMappingMethod = GetSToZMappingMethod();
            var sPlane = Normalize(poles, type, fcf1, fcf2, sToZMappingMethod == SToZMappingMethod.BilinearTransform);
            var zPlane = MapSPlaneToZPlane(sPlane, sToZMappingMethod);

            var tf = ComputeTransferFunction(zPlane);
            return ComputeIirFilterCoefficients(tf, ComputeGain(tf, type, fcf1, fcf2));
        }

        private static IOnlineFilter Create(IirFilterCoefficients coefficients)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Transforms the s-plane poles of the prototype filter into the s-plane poles and zeros for a filter
        /// with the specified pass type and cutoff frequencies.
        /// </summary>
        /// <param name="poles">The s-plane poles of the prototype LP filter.</param>
        /// <param name="type">The filter type (supports only low-pass, high-pass, band-pass and band-stop.</param>
        /// <param name="fcf1">
        /// The relative filter cutoff frequency for low-pass/high-pass, lower cutoff frequency for band-pass/band-stop.
        /// The cutoff frequency is specified relative to the sampling rate and must be between 0 and 0.5.
        /// </param>
        /// <param name="fcf2">
        /// Ignored for low-pass/high-pass, the relative upper cutoff frequency for band-pass/band-stop.
        /// The cutoff frequency is specified relative to the sampling rate and must be between 0 and 0.5.
        /// </param>
        /// <param name="preWarp">
        /// <see langword="true"/>true to enable prewarping of the cutoff frequencies (for a later bilinear transform from s-plane to z-plane),
        /// <see langword="false"/> to skip prewarping (for a later matched Z-transform).
        /// </param>
        /// <returns>The s-plane poles and zeros for the specific filter.</returns>
        private static PolesAndZeros Normalize(Complex[] poles, FilterKind type, double fcf1, double fcf2, bool preWarp)
        {
            bool fcf2IsRelevant = type == FilterKind.BandPass || type == FilterKind.BandStop;
            Debug.Assert(fcf1 > 0 && fcf1 < 0.5);
            Debug.Assert(!fcf2IsRelevant || (fcf2 > 0 && fcf2 < 0.5));

            int n = poles.Length;
            double fcf1Warped = Math.Tan(Math.PI * fcf1) / Math.PI;
            double fcf2Warped = fcf2IsRelevant ? Math.Tan(Math.PI * fcf2) / Math.PI : 0;
            double w1 = 2 * Math.PI * (preWarp ? fcf1Warped : fcf1);
            double w2 = 2 * Math.PI * (preWarp ? fcf2Warped : fcf2);

            if (type == FilterKind.LowPass)
            {
                return new PolesAndZeros
                {
                    Poles = poles.Select(x => x * w1).ToArray(),
                    Zeros = new Complex[0]
                };
            }

            if (type == FilterKind.HighPass)
            {
                var sPlane = new PolesAndZeros(n, n);

                for (int i = 0; i < n; ++i)
                    sPlane.Poles[i] = w1 / poles[i];

                return sPlane;
            }

            if (type == FilterKind.BandPass)
            {
                double w0 = Math.Sqrt(w1 * w2);
                double bw = w2 - w1;
                var sPlane = new PolesAndZeros(n * 2, n);

                for (int i = 0; i < n; ++i)
                {
                    var hba = poles[i] * (bw / 2);
                    var temp = Complex.Sqrt(1 - ((w0 / hba) * (w0 / hba)));
                    sPlane.Poles[i] = hba * (temp + 1);
                    sPlane.Poles[n + i] = hba * (1 - temp);
                }

                return sPlane;
            }

            if (type == FilterKind.BandStop)
            {
                double w0 = Math.Sqrt(w1 * w2);
                double bw = w2 - w1;
                var sPlane = new PolesAndZeros(n * 2, n * 2);

                for (int i = 0; i < n; ++i)
                {
                    var hba = (bw / 2) / poles[i];
                    var temp = Complex.Sqrt(1 - ((w0 / hba) * (w0 / hba)));
                    sPlane.Poles[i] = hba * (temp + 1);
                    sPlane.Poles[n + i] = hba * (1 - temp);
                }

                for (int i = 0; i < n; i++)
                {
                    sPlane.Zeros[i] = new Complex(0, w0);
                    sPlane.Zeros[n + i] = new Complex(0, -w0);
                }

                return sPlane;
            }

            throw new System.ComponentModel.InvalidEnumArgumentException("type", (int)type, typeof(FilterKind));
        }

        /// <devdoc>
        /// Maps the poles and zeros from the s-plane (<c>sPlane)</c>to the z-plane.
        /// </devdoc>
        private static PolesAndZeros MapSPlaneToZPlane(PolesAndZeros sPlane, SToZMappingMethod sToZMappingMethod)
        {
            switch (sToZMappingMethod)
            {
                case SToZMappingMethod.BilinearTransform:
                    {
                        return new PolesAndZeros
                        {
                            Poles = DoBilinearTransform(sPlane.Poles),
                            Zeros = Extend(DoBilinearTransform(sPlane.Zeros), sPlane.Poles.Length, new Complex(-1, 0))
                        };
                    }
                case SToZMappingMethod.MatchedZTransform:
                    {
                        return new PolesAndZeros
                        {
                            Poles = DoMatchedZTransform(sPlane.Poles),
                            Zeros = DoMatchedZTransform(sPlane.Zeros)
                        };
                    }
                default:
                    throw new System.ComponentModel.InvalidEnumArgumentException("sToZMappingMethod", (int)sToZMappingMethod, typeof(SToZMappingMethod));
            }
        }

        private static Complex[] DoBilinearTransform(Complex[] a)
        {
            return a.Select(x => (x + 2) / (2 - x)).ToArray();
        }

        private static Complex[] Extend(Complex[] a, int n, Complex fill)
        {
            if (a.Length >= n)
                return a;

            var result = new Complex[n];
            for (int i = 0; i < a.Length; ++i)
                result[i] = a[i];

            for (int i = a.Length; i < n; ++i)
                result[i] = fill;

            return result;
        }

        private static Complex[] DoMatchedZTransform(Complex[] a)
        {
            return a.Select(x => Complex.Exp(x)).ToArray();
        }

        /// <devdoc>
        /// Given the z-plane poles and zeros, compute the rational fraction (the top and bottom polynomial coefficients)
        /// of the filter transfer function in Z.
        /// </devdoc>
        private static Polynomials.RationalFraction ComputeTransferFunction(PolesAndZeros zPlane)
        {
            var topCoeffsComplex = Polynomials.Expand(zPlane.Zeros);
            var bottomCoeffsComplex = Polynomials.Expand(zPlane.Poles);

            // If the GetRealPart() conversion fails because the coffficients are not real numbers, the poles
            // and zeros are not complex conjugates.
            return new Polynomials.RationalFraction
            {
                Top = topCoeffsComplex.Select(x => GetRealPart(x)).ToArray(),
                Bottom = bottomCoeffsComplex.Select(x => GetRealPart(x)).ToArray(),
            };
        }

        private static double ComputeGain(Polynomials.RationalFraction tf, FilterKind type, double fcf1, double fcf2)
        {
            switch (type)
            {
                case FilterKind.LowPass:
                    return ComputeGainAt(tf, Complex.One); // At DC
                case FilterKind.HighPass:  // At Nyquist
                    return ComputeGainAt(tf, new Complex(-1, 0));
                case FilterKind.BandPass: // At center frequency
                    return ComputeGainAt(tf, Mathx.Expj(2 * Math.PI * ((fcf1 + fcf2) / 2)));
                case FilterKind.BandStop: // At sqrt( [gain at DC] * [gain at samplingRate/2]
                    return Math.Sqrt(ComputeGainAt(tf, Complex.One) * ComputeGainAt(tf, new Complex(-1, 0)));
                default:
                    throw new System.ComponentModel.InvalidEnumArgumentException("type", (int)type, typeof(FilterKind));
            }
        }

        private static double ComputeGainAt(Polynomials.RationalFraction tf, Complex w)
        {
            return Complex.Abs(Polynomials.Evaluate(tf, w));
        }

        /// <devdoc>
        /// Returns the IIR filter coefficients for a transfer function.
        /// Normalizes the coefficients so that a[0] is 1.
        /// </devdoc>
        private static IirFilterCoefficients ComputeIirFilterCoefficients(Polynomials.RationalFraction tf, double gain)
        {
            // Note that compared to the original C code by Tony Fisher the order of the A/B coefficients
            // is reverse and the A coefficients are negated.
            double scale = tf.Bottom[0];
            var coefficients = new IirFilterCoefficients();
            coefficients.A = tf.Bottom.Select(x => x / scale / gain).ToArray();
            coefficients.A[0] = 1;
            coefficients.B = tf.Top.Select(x => x / scale / gain).ToArray();
            
            return coefficients;
        }
       
        private static double GetRealPart(Complex number)
        {
            const double maximumImaginaryPart = 1E-10;

            double im = Math.Abs(number.Imaginary);
            if (im > maximumImaginaryPart && im > Math.Abs(number.Real) * maximumImaginaryPart)
            {
                string message = String.Format("The imaginary part of the complex number is not neglectable small for the conversion to a real number. real= {0} imaginary={1} epsilon={2}.",
                    number.Real, number.Imaginary, maximumImaginaryPart);

                throw new ArithmeticException(message);
            }

            return number.Real;
        }
    }
}
