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

namespace Bsa.Dsp.Filters
{
    /// <summary>
    /// Represents a filter design.
    /// </summary>
    public static class OnlineFilterDesign
    {
        /// <summary>
        /// Contains Infinite Impulse Response (IIR) filter designers.
        /// </summary>
        public static class Iir
        {
            /// <summary>
            /// Gets a Type I Chebyshev filter designer. It supports all filter types but <see cref="FilterType.AllPass"/>.
            /// </summary>
            /// <remarks>
            /// Filter is calculated using Fisher's method.
            /// It's widely used (in EEG, for example, is common when cutoff is far away from frequency range of interest).
            /// </remarks>
            public static readonly OnlineFilterDesigner ChebyshevI = new ChebyshevIOnlineFilterDesigner();

            /// <summary>
            /// Gets a Type II Chebyshev filter designer. It supports all filter types but <see cref="FilterType.AllPass"/>.
            /// </summary>
            /// <remarks>
            /// Filter is calculated using Fisher's method.
            /// It's widely used (in EEG, for example, is common when cutoff is far away from frequency range of interest).
            /// </remarks>
            public static readonly OnlineFilterDesigner ChebyshevII = new ChebyshevIIOnlineFilterDesigner();

            /// <summary>
            /// Gets a Butterworth filter designer. It supports all filter types but <see cref="FilterType.AllPass"/>, shelf and peak filters.
            /// </summary>
            /// <remarks>
            /// Fisher's method. This filter design is extremely common in EEG signal processing (especially for band-pass filters when inside
            /// frequency range of interest).
            /// </remarks>
            public static readonly OnlineFilterDesigner Butterworth = new ButterworthOnlineFilterDesigner();

            /// <summary>
            /// Gets a RBJ filter designer. It supports all filter types.
            /// </summary>
            /// <remarks>
            /// Very common in some niche applications and in audio processing for building equalizers. They perform especially well,
            /// for some type of data, when used to build a big cascade of filters (for example notches).
            /// Detailed description at http://www.musicdsp.org/files/Audio-EQ-Cookbook.txt.
            /// </remarks>
            public static readonly OnlineFilterDesigner Rbj = new RbjFilterDesigner();
        }

        /// <summary>
        /// Contains Finite Impulse Response (FIR) filter designers.
        /// </summary>
        public static class Fir
        {
            /// <summary>
            /// Gets the Parks/McClellan filter designer. This algorithm shapes a FIR Chebyshev filter. It supports
            /// <see cref="FilterType.LowPass"/>, <see cref="FilterType.HighPass"/> and <see cref="FilterType.BandPass"/>.
            /// </summary>
            /// <remarks>
            /// Very common and widely used implementation, decent overall performance it's multi-purpose.
            /// </remarks>
            public static readonly OnlineFilterDesigner ParksMcClellan = new ParksMcClellanOnlineFilterDesigner();
        }

        /// <summary>
        /// Gets the Savitzky/Golay filter designer. It supports only <see cref="FilterType.AllPass"/>.
        /// </summary>
        /// <remarks>
        /// This filter has better performance than a simple moving average (which is just a special case of this where <c>k=0</c> and <c>Y=a0</c>)
        /// for filtering of some signals, for example ECG (see also <em>Determination of Signal to Noise Ratio of Electrocardiograms Filtered by Band Pass
        /// and Savitzky-Golay Filters</em> by Dr. Monisha Chakrabortya and Shreya Das). Implementation based on
        /// original article http://pubs.acs.org/doi/abs/10.1021/ac60214a047.
        /// </remarks>
        public static readonly OnlineFilterDesigner SavitzkyGolay = new SavitzkyGolayFilterDesigner();

        // TODO: bessel, biquad and elliptic? Are used enough in this field to be worth of the effort? Elliptic is often used in SMR.
        // Note we may reuse some code from bessel filters for other IIR filters...
        // Butterworth with Exstrom's method?
    }
}
