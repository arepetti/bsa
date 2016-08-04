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

namespace Bsa.Dsp
{
    sealed class IirFilter : Disposable, IOnlineFilter
    {
        public IirFilter(double[] coefficients)
        {
            Debug.Assert(coefficients != null);
            Debug.Assert((coefficients.Length & 1) == 0);

            IsEnabled = true;
            _a = new double[coefficients.Length];
            _aBuffer = new double[coefficients.Length];
            _b = new double[coefficients.Length];
            _bBuffer = new double[coefficients.Length];
            
            _halfSize = coefficients.Length / 2;

            for (int i = 0; i < _halfSize; ++i)
            {
                _b[i] = _b[_halfSize + i] = coefficients[i];
                _a[i] = _a[_halfSize + i] = coefficients[_halfSize + i];
            }
        }

        public bool IsEnabled
        {
            get;
            set;
        }

        public double Process(double sample)
        {
            if (!IsEnabled)
                return sample;

            Debug.Assert(_bBuffer != null && _b != null);
            Debug.Assert(_aBuffer != null && _a != null);
            Debug.Assert(_offset >= 0 && _offset < _halfSize);

            _offset = _offset > 0 ? _offset - 1 : _halfSize - 1;

            _bBuffer[_offset] = sample;
            _aBuffer[_offset] = 0;

            double output = 0;

            for (int i = 0, j = _halfSize - _offset; i < _halfSize; ++i, ++j)
                output += _bBuffer[i] * _b[j];

            for (int i = 0, j = _halfSize - _offset; i < _halfSize; ++i, ++j)
                output -= _aBuffer[i] * _a[j];

            return _aBuffer[_offset] = output;
        }

        public void Reset()
        {
            Debug.Assert(_bBuffer != null);
            Debug.Assert(_aBuffer != null);

            Array.Clear(_bBuffer, 0, _bBuffer.Length);
            Array.Clear(_aBuffer, 0, _aBuffer.Length);
        }
       
        private readonly double[] _a, _b;
        private readonly double[] _aBuffer, _bBuffer;
        private readonly int _halfSize;
        private int _offset;
    }
}
