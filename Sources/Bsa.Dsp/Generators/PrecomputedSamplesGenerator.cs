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

namespace Bsa.Dsp.Generators
{
    /// <summary>
    /// Implementation of <see cref="SamplesGenerator"/> that uses a pre-computed array of
    /// samples as its output.
    /// </summary>
    public class PrecomputedSamplesGenerator : SamplesGenerator
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PrecomputedSamplesGenerator"/>.
        /// </summary>
        /// <param name="samples">
        /// Array of samples. Output consists of all these samples repeated in a loop.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="samples"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="samples"/> is an empty array (length is 0).
        /// </exception>
        public PrecomputedSamplesGenerator(double[] samples)
        {
            if (samples == null)
                throw new ArgumentNullException("samples");

            if (samples.Length == 0)
                throw new ArgumentException("Cannot use an empty array.", "samples");

            _samples = samples;
        }

        /// <summary>
        /// Generates next value.
        /// </summary>
        /// <returns>
        /// Generated value taken sequentially from looped input array.
        /// </returns>
        public override sealed double Next()
        {
            Debug.Assert(_samples != null && _samples.Length > 0);
            Debug.Assert(_currentIndex >= 0 && _currentIndex <= _samples.Length);

            if (_currentIndex == _samples.Length)
                _currentIndex = 0;

            return _samples[_currentIndex++];
        }

        /// <summary>
        /// Resets this generator to its initial state (next sample will be
        /// input array first sample).
        /// </summary>
        public override void Reset()
        {
            _currentIndex = 0;
        }

        private readonly double[] _samples;
        private int _currentIndex;
    }
}
