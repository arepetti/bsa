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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Numerics;

namespace Bsa.Hardware.Acquisition
{
    /// <summary>
    /// Additional arguments for the <see cref="AcquisitionDevice.Ohmeter"/> event.
    /// </summary>
    public sealed class OhmeterEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OhmeterEventArgs"/>.
        /// </summary>
        /// <param name="impedances">List of the measured impedances.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="impedances"/> is <see langword="null"/>.
        /// </exception>
        public OhmeterEventArgs(IEnumerable<PhysicalChannelImpedance> impedances)
        {
            if (impedances == null)
                throw new ArgumentNullException("impedances");

            Impedances = impedances;
        }

        /// <summary>
        /// Gets the list of measured impedances.
        /// </summary>
        /// <value>
        /// The list of measured impedances. This list may be a complete list with
        /// all input channels (<see cref="AcquisitionDevice{T}.Channels"/>) where
        /// unknown/unavailable impedances have one or more <see langword="null"/>
        /// in <see cref="PhysicalChannelImpedance.Impedances"/> or a partial list
        /// which contains only channels where impedance is available/known.
        /// </value>
        public IEnumerable<PhysicalChannelImpedance> Impedances
        {
            get;
            private set;
        }
    }
}
