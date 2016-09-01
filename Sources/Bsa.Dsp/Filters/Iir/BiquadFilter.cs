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

namespace Bsa.Dsp.Filters.Iir
{
    sealed class BiquadFilter : Disposable, IOnlineFilter
    {
        public BiquadFilter(IirFilterCoefficients coefficients)
        {
            Debug.Assert(coefficients != null);
            Debug.Assert(coefficients.B != null && coefficients.B.Length == 3);
            Debug.Assert(coefficients.A != null && coefficients.A.Length == 3);

            IsEnabled = true;
            
            // From http://www.musicdsp.org/files/Audio-EQ-Cookbook.txt:
            //
            //            b0 + b1*z^-1 + b2*z^-2
            //    H(z) = ------------------------                                  (Eq 1)
            //            a0 + a1*z^-1 + a2*z^-2
            //
            // This shows 6 coefficients instead of 5 so, depending on your architechture,
            // you will likely normalize a0 to be 1 and perhaps also b0 to 1 (and collect
            // that into an overall gain coefficient).  Then your transfer function would
            // look like:
            //
            //            (b0/a0) + (b1/a0)*z^-1 + (b2/a0)*z^-2
            //    H(z) = ---------------------------------------                   (Eq 2)
            //               1 + (a1/a0)*z^-1 + (a2/a0)*z^-2
            //
            // or

            //                      1 + (b1/b0)*z^-1 + (b2/b0)*z^-2
            //    H(z) = (b0/a0) * ---------------------------------               (Eq 3)
            //                      1 + (a1/a0)*z^-1 + (a2/a0)*z^-2
            //
            //
            // The most straight forward implementation would be the "Direct Form 1"
            // (Eq 2):

            //    y[n] = (b0/a0)*x[n] + (b1/a0)*x[n-1] + (b2/a0)*x[n-2]
            //                        - (a1/a0)*y[n-1] - (a2/a0)*y[n-2]            (Eq 4)
            
            _b0 = coefficients.B[0] / coefficients.A[0];
            _b1 = coefficients.B[1] / coefficients.A[0];
            _b2 = coefficients.B[2] / coefficients.A[0];
            _a1 = coefficients.A[1] / coefficients.A[0];
            _a2 = coefficients.A[2] / coefficients.A[0];
        }

        public bool IsEnabled
        {
            get;
            set;
        }

        public double Process(double value)
        {
            if (!IsEnabled)
                return value;

            _in2 = _in1;
            _in1 = value;
            _out2 = _out1;

            return _out1 = _b0 * value + _b1 * _in1 + _b2 * _in2 - _a1 * _out1 - _a2 * _out2;
        }

        public void Reset()
        {
            _out1 = _out2 = _in1 = _in2 = 0;
        }

        private readonly double _b0, _b1, _b2, _a1, _a2;
        private double _out1, _out2, _in1, _in2;
    }
}
