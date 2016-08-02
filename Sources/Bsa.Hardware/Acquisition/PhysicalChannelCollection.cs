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
using System.Linq;
using System.Collections.ObjectModel;

namespace Bsa.Hardware.Acquisition
{
    /// <summary>
    /// Represents a collection of <see cref="PhysicalChannel"/>.
    /// </summary>
    public sealed class PhysicalChannelCollection<T> : SealableCollection<T>
        where T : PhysicalChannel
    {
        /// <summary>
        /// Determines if this collection contains a channel with specified ID.
        /// </summary>
        /// <param name="channelId">ID of the channel to search for.</param>
        /// <returns>
        /// <see langword="true"/> if this collection contains a channel with ID <see cref="PhysicalChannel.Id"/>
        /// equals to specified id <paramref name="channelId"/>).
        /// </returns>
        public bool Contains(Guid channelId)
        {
            return Items.FirstOrDefault(x => x.Id == channelId) != null;
        }

        /// <summary>
        /// Returns the channel with the specified ID.
        /// </summary>
        /// <param name="channelId">ID of the channel to search for.</param>
        /// <returns>
        /// The channel (of type <typeparamref name="T"/>) with ID <see cref="PhysicalChannel.Id"/>
        /// equals to specified id <paramref name="channelId"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If this collection does not contain any channel with ID <paramref name="channelId"/>.
        /// </exception>
        public T this[Guid channelId]
        {
            get { return Items.First(x => x.Id == channelId); }
        }
    }
}
