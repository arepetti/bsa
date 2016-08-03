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
using System.ComponentModel;
using System.Diagnostics;

namespace Bsa.Dsp.Generators
{
    static class WaveformFactory
    {
        public static double[] Create(WaveformDescription waveform, int chunckLength)
        {
            Debug.Assert(waveform != null);

            if (waveform.Waveform == Waveform.Dc)
                return CreateDc(waveform, chunckLength);

            if (waveform.Waveform == Waveform.Sine)
                return CreateSine(waveform, chunckLength);

            if (waveform.Waveform == Waveform.Impulse)
                return CreateImpulse(waveform, chunckLength);

            throw new InvalidEnumArgumentException("waveform.Waveform", (int)waveform.Waveform, typeof(Waveform));
        }

        private static double[] CreateDc(WaveformDescription waveform, int chunckLength)
        {
            Debug.Assert(waveform != null);
            Debug.Assert(chunckLength > 0);
            
            double[] samples = new double[chunckLength];
            double value = waveform.Amplitude + waveform.Offset;

            for (int i = 0; i < samples.Length; ++i)
                samples[i] = value;

            return samples;
        }

        private static double[] CreateSine(WaveformDescription waveform, int chunckLength)
        {
            Debug.Assert(waveform != null);
            Debug.Assert(waveform.SamplingRate > 0);
            Debug.Assert((int)waveform.SamplingRate == chunckLength, "Generation of epochs longer than one second is not currently supported.");

            if (waveform.Frequency == 0)
                return CreateDc(waveform, chunckLength);

            double[] samples = new double[chunckLength];
            double n = (waveform.Frequency / waveform.SamplingRate) * (Math.PI * 2);

            for (int i = 0; i < samples.Length; ++i)
                samples[i] = waveform.Offset + waveform.Amplitude * Math.Sin(waveform.Phase + i * n);

            return samples;
        }

        /// <devdoc>
        /// If chunck length is higher than sampling rate then then only first one second epoch
        /// will contain the impulse.
        /// </devdoc>
        private static double[] CreateImpulse(WaveformDescription waveform, int chunckLength)
        {
            Debug.Assert(waveform != null);
            Debug.Assert(waveform.SamplingRate > 0);

            double[] samples = new double[(int)waveform.SamplingRate];
            int impulseIndex = (int)((waveform.Phase / (Math.PI * 2)) * waveform.SamplingRate);

            for (int i = 0; i < samples.Length; ++i)
                samples[i] = waveform.Offset + (i == impulseIndex ? waveform.Amplitude : 0);

            return samples;
        }
    }
}
