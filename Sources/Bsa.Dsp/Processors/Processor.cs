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
using System.Collections.Generic;

namespace Bsa.Dsp.Processors
{
    /// <summary>
    /// Base class for a <see cref="IOnlineProcessor"/>.
    /// </summary>
    public abstract class Processor : Disposable, IOnlineProcessor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Processor"/>.
        /// </summary>
        protected Processor()
        {
            IsEnabled = true;
        }

        /// <summary>
        /// Indicates whether this processor is enabled.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this processor is enabled. When disabled input samples
        /// must flow unchanged through the processor preserving the same delay.
        /// Default value is  <see langword="true"/>.
        /// </value>
        public bool IsEnabled
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
        public double Process(double sample)
        {
            ThrowIfDisposed();

            if (IsEnabled)
                return ProcessCore(sample);

            return sample;
        }

        /// <summary>
        /// Reset processor's state (when applicable).
        /// </summary>
        public virtual void Reset()
        {
            ThrowIfDisposed();
        }

        /// <summary>
        /// Processes specified sample.
        /// </summary>
        /// <param name="sample">Sample to process.</param>
        /// <returns>
        /// Specified sample processed by this <c>Processor</c>.
        /// </returns>
        protected abstract double ProcessCore(double sample);
    }
}
