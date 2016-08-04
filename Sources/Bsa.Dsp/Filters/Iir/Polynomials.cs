//
// Original Java implementation (http://svn.source-code.biz/dsp-java/trunk/src/main/java/biz/source_code/dsp/math/PolynomialUtils.java).
// Java version license is:
//
// Copyright 2013 Christian d'Heureuse, Inventec Informatik AG, Zurich, Switzerland
// www.source-code.biz, www.inventec.ch/chdh
//
// This module is multi-licensed and may be used under the terms
// of any of the following licenses:
//
//  EPL, Eclipse Public License, V1.0 or later, http://www.eclipse.org/legal
//  LGPL, GNU Lesser General Public License, V2.1 or later, http://www.gnu.org/licenses/lgpl.html
//
// Please contact the author if you need another license.
// This module is provided "as is", without warranties of any kind.
//
//
// C# porting license:
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
using System.Numerics;

namespace Bsa.Dsp.Filters.Iir
{
    /// <devdoc>
    /// Plynomial utility routines.
    /// In this module, polynomial coefficients are stored in arrays ordered in descending powers.
    /// When an array <c>a[]</c> contains the coefficients, the polynomial has the following form:
    /// <c>a[0] * x^n + a[1] * x^(n-1) + ... a[n-1] * x + a[n]</c>.
    /// </devdoc>
    static class Polynomials
    {
        /// <devdoc>
        /// Structure for the coefficients of a rational fraction (a fraction of two polynomials) with real coefficients.
        /// http://en.wikipedia.org/wiki/Algebraic_fraction#Rational_fractions.
        /// </devdoc>
        public class RationalFraction
        {
            public double[] Top { get; set; }
            public double[] Bottom { get; set; }
        }

        public static Complex Evaluate(double[] a, Complex x)
        {
            var sum = new Complex(a[0], 0);

            for (int i = 1; i < a.Length; ++i)
                sum = sum * x + a[i];

            return sum;
        }

        public static Complex Evaluate(RationalFraction f, Complex x)
        {
            return Evaluate(f.Top, x) / Evaluate(f.Bottom, x);
        }

        public static double[] Multiply(double[] a1, double[] a2)
        {
            int n1 = a1.Length - 1;
            int n2 = a2.Length - 1;
            int n3 = n1 + n2;

            var result = new double[n3 + 1];

            for (int i = 0; i <= n3; ++i)
            {
                double t = 0;
                int p1 = Math.Max(0, i - n2);
                int p2 = Math.Min(n1, i);

                for (int j = p1; j <= p2; ++j)
                    t += a1[n1 - j] * a2[n2 - i + j];

                result[n3 - i] = t;
            }

            return result;
        }

        public static Complex[] Multiply(Complex[] a1, Complex[] a2)
        {
            int n1 = a1.Length - 1;
            int n2 = a2.Length - 1;
            int n3 = n1 + n2;

            var result = new Complex[n3 + 1];

            for (int i = 0; i <= n3; ++i)
            {
                Complex t = Complex.Zero;
                int p1 = Math.Max(0, i - n2);
                int p2 = Math.Min(n1, i);

                for (int j = p1; j <= p2; j++)
                    t = t + (a1[n1 - j] * a2[n2 - i + j]);

                result[n3 - i] = t;
            }

            return result;
        }

        /// <devdoc>
        /// Forward deflation of a polynomial with a known zero. Divides the polynomial with coefficients <c>a[]</c> by <c>(x - z)</c>,
        /// where <c>z</c> is a zero of the polynomial.
        /// <c>eps</c> is the maximum value allowed for the absolute real and imaginary parts of the remainder, or 0 to ignore the reminder.
        /// </devdoc>
        public static Complex[] Deflate(Complex[] a, Complex z, double eps)
        {
            int n = a.Length - 1;

            var result = new Complex[n];
            result[0] = a[0];

            for (int i = 1; i < n; i++)
                result[i] = (z * result[i - 1]) + a[i];

            Complex remainder = (z * result[n - 1]) + a[n];

            if (eps > 0 && (Math.Abs(remainder.Real) > eps || Math.Abs(remainder.Imaginary) > eps))
                throw new ArithmeticException("Polynom deflatation failed, remainder = " + remainder + ".");

            return result;
        }

        /// <devdoc>
        /// Computes the coefficients of a polynomial from it's complex zeros.
        /// The polynomial formula is: <c>(x - zero[0]) * (x - zero[1]) * ... (x - zero[n - 1])</c>.
        /// </devdoc>
        public static Complex[] Expand(Complex[] zeros)
        {
            if (zeros.Length == 0)
                return new Complex[] { Complex.One };

            // Start with (x - zeros[0])
            var result = new Complex[] { Complex.One, Complex.Negate(zeros[0]) };

            // Multiply factor (x - zeros[i]) into coefficients
            for (int i = 1; i < zeros.Length; i++)
                result = Multiply(result, new Complex[] { Complex.One, Complex.Negate(zeros[i]) });

            return result;
        }
    }
}
