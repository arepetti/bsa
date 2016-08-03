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
// along with BSA-F.  If not, see <http://www.gnu.org/licenses/>.
//

using System;

namespace Bsa.Dsp
{
    /// <summary>
    /// Interface implemented by online processors whose process a samples stream one by one.
    /// </summary>
    /// <remarks>
    /// Note that processors and filters are different because processors are strictly stateless:
    /// you can imagine <c>Amplifier</c>, <c>Inverter</c> and <c>Rectifier</c> processors. Processors
    /// may also be used to <em>change</em> signal (for exmaple to perform a conversion from <em>raw</em>
    /// values acquired by hardware device (expressed in the range of input channel) to generic
    /// abstract samples expressed in another unit of measure (or to perform linearization, if required).
    /// </remarks>
    public interface IOnlineProcessor : IDisposable
    {
        /// <summary>
        /// Indicates whether this processor is enabled.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this processor is enabled. When disabled input samples
        /// must flow unchanged through the processor (preserving the same delay but not any phase shift).
        /// Default value must always be <see langword="true"/>.
        /// Note that enabling/disabling a filter may introduce a discontinuity in its response.
        /// </value>
        bool IsEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Processes specified sample.
        /// </summary>
        /// <param name="sample">Sample to process.</param>
        /// <returns>
        /// Specified sample processed by this <c>IOnlineProcessor</c>.
        /// </returns>
        double Process(double sample);
    }
}
