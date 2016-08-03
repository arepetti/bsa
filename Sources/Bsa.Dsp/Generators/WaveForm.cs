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
    /// Represents the shape of a generated waveform.
    /// </summary>
    public enum Waveform
    {
        /// <summary>
        /// DC (constant) signal with amplitude given by the sum of waveform amplitude
        /// <see cref="WaveformDescription.Amplitude"/> and base DC offset <see cref="WaveformDescription.Offset"/>.
        /// <c>Phase</c>, <c>Frequency</c> and <c>SamplingRate</c> parameters are simply ignored.
        /// </summary>
        Dc,

        /// <summary>
        /// Sine waveform with amplitude given by the sum of waveform amplitude
        /// <see cref="WaveformDescription.Amplitude"/> and base DC offset <see cref="WaveformDescription.Offset"/>
        /// (note that amplitude given <c>WaveformDescriptor.Amplitude</c> will be both positive and negative).
        /// <see cref="WaveformDescription.Phase"/> is the phase (between 0 and 2π) of this sine (use this
        /// to generate a cosine waveform).
        /// </summary>
        Sine,

        /// <summary>
        /// Single impulse with amplitude given by the sum of waveform amplitude
        /// <see cref="WaveformDescription.Amplitude"/> and base DC offset <see cref="WaveformDescription.Offset"/>
        /// centered in position given by <see cref="WaveformDescription.Phase"/> (between 0 and 2π).
        /// <c>Frequency</c> parameter is ignored.
        /// </summary>
        Impulse,
    }
}
