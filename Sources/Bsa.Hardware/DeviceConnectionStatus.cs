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

namespace Bsa.Hardware
{
    /// <summary>
    /// Specifies the connection status of a <see cref="Device"/>.
    /// </summary>
    public enum DeviceConnectionStatus
    {
        /// <summary>
        /// Device is currently disconnected (or it has never been connected).
        /// </summary>
        Disconnected,

        /// <summary>
        /// Connection to device is in progress. When connection is in this state a disconnection cannot be performed
        /// until connection is completed (successfully or not).
        /// </summary>
        Connecting,

        /// <summary>
        /// Device is currently connected.
        /// </summary>
        Connected,

        /// <summary>
        /// Disconnection is in progress. When connection is in this state a new connection cannot be initiated
        /// until disconnection completed (successfully or not).
        /// </summary>
        Disconnecting,

        /// <summary>
        /// Connection to device has been attempted but it failed.
        /// </summary>
        Error
    }
}
