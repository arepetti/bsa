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
    /// Contains the description of a waveform to generate.
    /// </summary>
    public sealed class WaveformDescription
    {
        /// <summary>
        /// The maximum sampling rate that can be used to generate a waveform.
        /// </summary>
        /// <devdoc>
        /// This is the maximum because <see cref="WaveGenerator"/> uses <see cref="PrecomputedSamplesGenerator "/>
        /// to precompute an array for one second of data, 2 GB is maximum array size.
        /// </devdoc>
        public const double MaximumSamplingRate = 2147483647;

        /// <summary>
        /// Represents a phase shift of 90° (π/2). You may use this value for <see cref="Phase"/>.
        /// </summary>
        public const double Phase90 = Math.PI / 2;

        /// <summary>
        /// Represents a phase shift of 180° (π). You may use this value for <see cref="Phase"/>.
        /// </summary>
        public const double Phase180 = Math.PI;

        /// <summary>
        /// Represents a phase shift of 270° (3/2 π). You may use this value for <see cref="Phase"/>.
        /// </summary>
        public const double Phase270 = Math.PI * 1.5;

        /// <summary>
        /// Gets/sets the type of waveform to generate.
        /// </summary>
        /// <value>
        /// The type to waveform to generate, according to this value some
        /// other properties may be unused or have different meanings. Default value
        /// is <see cref="Bsa.Dsp.Generators.Waveform.Dc"/>.
        /// </value>
        public Waveform Waveform
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the sampling rate of the waveform to generate.
        /// </summary>
        /// <value>
        /// The sampling rate (in Hertz) of the waveform to generate. Default value
        /// is 100.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If value is <see cref="Double.NaN"/>, <see cref="Double.PositiveInfinity"/>,
        /// less or equal than zero, higher than <see cref="MaximumSamplingRate"/>.
        /// </exception>
        /// <remarks>
        /// This value is ignored for <see cref="Bsa.Dsp.Generators.Waveform.Dc"/>.
        /// </remarks>
        public double SamplingRate
        {
            get { return _samplingRate; }
            set
            {
                if (value <= 0 || value > MaximumSamplingRate || Double.IsNaN(value) || Double.IsPositiveInfinity(value))
                    throw new ArgumentOutOfRangeException();

                _samplingRate = value;
            }
        }

        /// <summary>
        /// Gets/sets the frequency (in Hertz) of the generated waveform.
        /// </summary>
        /// <value>
        /// The frequency in Hertz of the generated waveform. Default value is 1.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If value is <see cref="Double.NaN"/>, <see cref="Double.PositiveInfinity"/>
        /// or less than zero.
        /// </exception>
        /// <remarks>
        /// This value is ignored for <see cref="Bsa.Dsp.Generators.Waveform.Dc"/>
        /// and <see cref="Bsa.Dsp.Generators.Waveform.Impulse"/>.
        /// </remarks>
        public double Frequency
        {
            get { return _frequency; }
            set
            {
                if (value < 0 || Double.IsNaN(value) || Double.IsPositiveInfinity(value))
                    throw new ArgumentOutOfRangeException();

                _frequency = value;
            }
        }

        /// <summary>
        /// Gets/sets the amplitude of the generated waveform.
        /// </summary>
        /// <value>
        /// Amplitude of the generated waveform. This value has not unit of measure then
        /// unit of measure for samples depends on how users will handle them.
        /// Default value is 1.
        /// </value>
        public double Amplitude
        {
            get { return _amplitude; }
            set { _amplitude = value; }
        }

        /// <summary>
        /// Offset for generated waveform.
        /// </summary>
        /// <value>
        /// A constant offset added to <see cref="Amplitude"/> for all samples in the generated
        /// waveform. Default value is 0.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If value is <see cref="Double.NaN"/>, <see cref="Double.PositiveInfinity"/>
        /// or <see cref="Double.NegativeInfinity"/>.
        /// </exception>
        public double Offset
        {
            get { return _offset; }
            set
            {
                if (Double.IsNaN(value) || Double.IsInfinity(value))
                    throw new ArgumentOutOfRangeException();

                _offset = value;
            }
        }

        /// <summary>
        /// Gets/sets the phase of generated waveform.
        /// </summary>
        /// <value>
        /// Phase (between 0 and 2π) of the generated waveform. Default value is 0.
        /// </value>
        /// <remarks>
        /// This value is ignored for <see cref="Bsa.Dsp.Generators.Waveform.Dc"/>.
        /// </remarks>
        /// <seealso cref="Phase90"/>
        /// <seealso cref="Phase180"/>
        /// <seealso cref="Phase270"/>
        public double Phase
        {
            get { return _phase; }
            set
            {
                if (value < 0 || value > Math.PI * 2 || Double.IsNaN(value))
                    throw new ArgumentOutOfRangeException();

                _phase = value;
            }
        }

        private double _samplingRate = 100, _frequency = 1, _amplitude = 1, _phase, _offset;
    }
}
