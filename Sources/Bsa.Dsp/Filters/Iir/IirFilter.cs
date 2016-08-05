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
    sealed class IirFilter : Disposable, IOnlineFilter
    {
        public IirFilter(IirFilterCoefficients coefficients)
        {
            Debug.Assert(coefficients != null);
            Debug.Assert(coefficients.A.Length > 0 && coefficients.B.Length > 0 && coefficients.A[0] == 1.0);

            IsEnabled = true;

           _a = coefficients.A;
           _b = coefficients.B;
           _nb = _b.Length - 1;
           _na = _a.Length - 1;
           _buf1 = new double[_nb];
           _buf2 = new double[_na];
        }

        public bool IsEnabled
        {
            get;
            set;
        }

        public double Process(double sample)
        {
            Debug.Assert(_a != null && _b != null);
            Debug.Assert(_buf1 != null && _buf2 != null);
            Debug.Assert(_buf1.Length == _nb && _buf2.Length == _na);

            if (!IsEnabled)
                return sample;

            double acc = _b[0] * sample;
            for (int j = 1; j <= _nb; j++)
            {
                int p = (_pos1 + _nb - j) % _nb;
                acc += _b[j] * _buf1[p];
            }
            for (int j = 1; j <= _na; j++)
            {
                int p = (_pos2 + _na - j) % _na;
                acc -= _a[j] * _buf2[p];
            }
            if (_nb > 0)
            {
                _buf1[_pos1] = sample;
                _pos1 = (_pos1 + 1) % _nb;
            }
            if (_na > 0)
            {
                _buf2[_pos2] = acc;
                _pos2 = (_pos2 + 1) % _na;
            }
            return acc;
        }

        public void Reset()
        {
            Debug.Assert(_buf1 != null);
            Debug.Assert(_buf2 != null);

            Array.Clear(_buf1, 0, _buf1.Length);
            Array.Clear(_buf2, 0, _buf2.Length);
        }       

        private readonly int _nb, _na;
        private readonly double[] _a, _b;

        private readonly double[] _buf1, _buf2;
        private int _pos1, _pos2;
   }
}
