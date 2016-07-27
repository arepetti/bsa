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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

namespace Bsa.Hardware.Acquisition
{
    /// <summary>
    /// Represents the impedances read for a specific channel.
    /// </summary>
    [Serializable]
    public sealed class PhysicalChannelImpedance
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PhysicalChannelImpedance"/>.
        /// </summary>
        /// <param name="channelId">ID of the channel associated with these impedances.</param>
        /// <param name="impedances">
        /// List of impedances for a specific channel. List may be of one (for monopolar inputs) or two (for bipolar
        /// inputs) impedances but it cannot be empty (you can however specify <see langword="null"/> if impedance value
        /// is unknown/unavailable).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="impedances"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If list is not of one or two impedances.
        /// </exception>
        public PhysicalChannelImpedance(Guid channelId, params Complex?[] impedances)
        {
            if (impedances == null)
                throw new ArgumentNullException("impedances");

            if (impedances.Length == 0)
                throw new ArgumentException("At least one impedance must be specified, if value is unknown you can specify null.", "impedances");

            if (impedances.Length > 2)
                throw new ArgumentException("At most you can specify two impedances for bipolar inputs.", "impedances");

            ChannelId = channelId;
            Impedances = new ReadOnlyCollection<Complex?>(impedances);
        }

        /// <summary>
        /// Gets the ID of the channel associated with these impedances.
        /// </summary>
        /// <value>
        /// The ID of the channel associated with these impedances.
        /// </value>
        public Guid ChannelId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of impedances for the channel with ID <see cref="ChannelId"/>.
        /// </summary>
        /// <value>
        /// The list of impedances for the channel with ID <see cref="ChannelId"/>. This list
        /// may be composed of one (for monopolar inputs) or two elements (for bipolar inputs) and
        /// it may contain <see langword="null"/> if impedance is unknown/unavailable.
        /// </value>
        public IReadOnlyList<Complex?> Impedances
        {
            get;
            private set;
        }
    }
}
