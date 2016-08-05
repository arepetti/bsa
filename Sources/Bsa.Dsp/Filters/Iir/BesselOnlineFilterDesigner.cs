//
// Original algorithm and code from T. Fisher (http://www-users.cs.york.ac.uk/~fisher/),
// see also Fisher: "Digital signal processing of Decca Navigator radionavigation signals" (1999).
//
//
// This C# code is an adaption of Java port from original C code by Christian d'Heureuse (2013).
// Java version is multi-licensed under EPL 1.0 (or later) and L-GPL 2.1 (or later).
// More details at http://svn.source-code.biz/dsp-java/trunk/src/main/java/biz/source_code/dsp/filter/BesselFilterDesign.java
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
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Bsa.Dsp.Filters.Iir
{
    sealed class BesselOnlineFilterDesigner : FisherMethodFilterDesigner
    {
        protected override Complex[] GetPoles(int filterOrder, double ripple)
        {
            double[] polynomialCoefficients = ComputePolynomialCoefficients(filterOrder)
                .Reverse()
                .ToArray();

            double scalingFactor = FindFrequencyScalingFactor(polynomialCoefficients);
            return Polynomials.JenkinsTraubRootFinder.FindRoots(polynomialCoefficients)
                .Select(x => x / scalingFactor)
                .ToArray();
        }

        /// <devdoc>
        /// Returns the polynomial coefficients for the Bessel polynomial of the given order.
        /// It returns an array <c>a[]</c> with the coefficients, ordered in descending powers, for:
        /// <c>a[0] * x^n + a[1] * x^(n-1) + ... a[n-1] * x + a[n]</c>.
        /// Details at: http://en.wikipedia.org/wiki/Bessel_polynomials.
        /// </devdoc>
        private static double[] ComputePolynomialCoefficients(int order)
        {
            double m = 1;
            for (int i = 1; i <= order; ++i)
                m = m * (order + i) / 2;

            double[] a = new double[order + 1];
            a[0] = m;
            a[order] = 1;
            
            for (int i = 1; i < order; ++i)
                a[i] = a[i - 1] * 2 * (order - i + 1) / (2 * order - i + 1) / i;

            return a;
        }

        /// <devdoc>
        ///  This method uses appoximation to find the frequency for a given gain.
        ///  It is used to find the 3dB cutoff frequency, which is then used as the scaling factor
        ///  for the frequency normalization of the filter. Input is a normalized gain and output is
        ///  a relative (in range 0...0.5) frequency.
        /// </devdoc>
        private static double FindFrequencyForGain(double[] coefficients, double gain)
        {
            const double eps = 1E-15;
            
            if (gain > (1 - 1E-6) || gain < 1E-6)
                throw new ArgumentOutOfRangeException("gain");

            // Find starting point for lower frequency.
            double lowerBoundOfSearchRange = 1;
            int lowerBoundIterationCount = 0;
            while (ComputeGain(coefficients, lowerBoundOfSearchRange) < gain)
            {
                lowerBoundOfSearchRange /= 2;
                if (++lowerBoundIterationCount > 100)
                    throw new ArithmeticException("Too many iterations.");
            }

            // Find starting point for upper frequency.
            double upperBoundOfSearchRange = 1;
            int upperBoundIterationCount = 0;
            while (ComputeGain(coefficients, upperBoundOfSearchRange) > gain)
            {
                upperBoundOfSearchRange *= 2;
                if (++upperBoundIterationCount > 100)
                    throw new ArithmeticException("Too many iterations.");
            }

            // Approximation loop:
            int approximationIterationCount = 0;
            while (true)
            {
                if (upperBoundOfSearchRange - lowerBoundOfSearchRange < eps)
                    break;

                double wm = (upperBoundOfSearchRange + lowerBoundOfSearchRange) / 2;
                double gm = ComputeGain(coefficients, wm);
                
                if (gm > gain)
                    lowerBoundOfSearchRange = wm;
                else
                    upperBoundOfSearchRange = wm;

                if (++approximationIterationCount > 1000)
                    throw new ArithmeticException("No convergence.");

                return lowerBoundOfSearchRange;
            }

            throw new ArithmeticException("No convergence.");
        }

        /// <devdoc>
        /// Returns the frequency normalization scaling factor for a Bessel filter.
        /// This factor is used to normalize the filter coefficients so that the gain at the relative frequency 1 is -3dB.
        /// (To be exact, we use 1/sqrt(2) instead of -3dB).
        /// </devdoc>
        private static double FindFrequencyScalingFactor(double[] polyCoeffs)
        {
            return FindFrequencyForGain(polyCoeffs, 1 / Math.Sqrt(2));
        }

        private static double ComputeGain(double[] besselPolynomialCoefficients, double relativeFrequencyW)
        {
            return Complex.Abs(TransferFunction(besselPolynomialCoefficients, new Complex(0, relativeFrequencyW)));
        }

        /// <devdoc>
        /// Evaluates the transfer function of a Bessel filter. The gain is normalized to 1 for DC.
        /// It returns a complex number that corresponds to the gain and phase of the filter output.
        /// </devdoc>
        private static Complex TransferFunction(double[] besselPolynomialCoefficients, Complex frequency)
        {
            var f = new Polynomials.RationalFraction
            {
                Top = new double[] { besselPolynomialCoefficients[besselPolynomialCoefficients.Length - 1] }, // to normalize gain at DC
                Bottom = besselPolynomialCoefficients
            };

            return Polynomials.Evaluate(f, frequency);
        }
    }
}
