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
    /// Additional arguments for the <see cref="AcquisitionDevice.Data"/> event.
    /// </summary>
    public sealed class DataEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataEventArgs"/>.
        /// </summary>
        /// <param name="data">Acquired samples.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="data"/> is <see langword="null"/>.
        /// </exception>
        public DataEventArgs(DataPacket data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Data = data;
        }

        /// <summary>
        /// Gets the acquired samples.
        /// </summary>
        /// <value>
        /// The acquired samples.
        /// </value>
        public DataPacket Data
        {
            get;
            private set;
        }
    }
}
