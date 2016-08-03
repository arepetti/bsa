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

namespace Bsa.Dsp.Generators
{
    /// <summary>
    /// Synthesize a waveform with the specified properties.
    /// </summary>
    public sealed class WaveGenerator : PrecomputedSamplesGenerator
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WaveGenerator"/>.
        /// </summary>
        /// <param name="waveform">
        /// Description of the waveform to synthesize. Note that, for <see cref="Waveform.Sine"/>, if sampling rate <see cref="WaveformDescription.SamplingRate"/>
        /// is not an integer value then there may be a visible error at the end of each second in the generated waveform.
        /// Also note that, because waveform is synthesized when this object is constructed, there may be an <see cref="OutOfMemoryException"/>
        /// if sampling rate is extremely high (one full epoch of one second will be generated for all waveforms but <see cref="Waveform.Dc"/>)
        /// or <see cref="ArgumentOutOfRangeException"/> when you set <c>WaveformDescription.SamplingRate</c> if required value is higher
        /// than <see cref="WaveformDescription.MaximumSamplingRate"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="waveform"/> is <see langword="null"/>.
        /// </exception>
        public WaveGenerator(WaveformDescription waveform)
            : base(WaveformFactory.Create(waveform, ResolveRequiredChunkLength(waveform)))
        {
        }

        private static int ResolveRequiredChunkLength(WaveformDescription waveform)
        {
            if (waveform == null)
                throw new ArgumentNullException("waveform");

            if (waveform.Waveform == Waveform.Dc)
                return 1;

            return (int)waveform.SamplingRate;
        }
    }
}
