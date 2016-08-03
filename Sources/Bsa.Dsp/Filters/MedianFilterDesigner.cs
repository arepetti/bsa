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

namespace Bsa.Dsp.Filters
{
    sealed class MedianFilterDesigner : OnlineFilterDesigner
    {
        protected internal override IOnlineFilter CreateOther(FilterDesignSettings settings)
        {
            int windowSize = settings.Order > 0 ? settings.Order : MedianFilterDesignSettings.DefaultWindowSize;

            var medianSettings = settings as MedianFilterDesignSettings;
            if (medianSettings != null)
                windowSize = medianSettings.WindowSize;

            return new Filter(windowSize);
        }

        private sealed class Filter : IOnlineFilter
        {
            public Filter(int windowSize)
            {
                Debug.Assert(windowSize > 0);

                _buffer = new double[windowSize];
            }

            public void Reset()
            {
                _offset = 0;
            }

            public double Process(double sample)
            {
                Debug.Assert(_buffer != null);

                if (_offset == _buffer.Length)
                    AppendSample(sample);
                else
                    _buffer[_offset++] = sample;
                
                return Median(_offset);
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            private readonly double[] _buffer;
            private int _offset;

            private void AppendSample(double sample)
            {
                Array.Copy(_buffer, 1, _buffer, 0, _buffer.Length - 1);
                _buffer[_buffer.Length - 1] = sample;
            }

            private double Median(int numberOfBufferedSamples)
            {
                // Let's assume we can use a simple median (if even just pick one sample when they're even).
                // It's reasonable because if we use this filter we want to preserve sharp edges and
                // averaging two samples won't do it.
                return _buffer[(numberOfBufferedSamples - 1) / 2];
            }
        }
    }
}
