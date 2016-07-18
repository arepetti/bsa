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
// You should have received a copy of the GNU Lesse General Public License
// along with BSA-F.  If not, see <http://www.gnu.org/licenses/>.
//

using System;

namespace Bsa.Hardware
{
    /// <summary>
    /// Contains constants used for well-known hardware errors.
    /// </summary>
    public static class HardwareErrorCodes
    {
        /// <summary>
        /// Contains constants for well-known hardware errors for class <see cref="HardwareErrorClass.Generic"/>
        /// </summary>
        public static class Generic
        {
            /// <summary>
            /// Unknown error.
            /// </summary>
            public const ushort UnknownError = 0;
        }

        /// <summary>
        /// Contains constants for well-known hardware errors for class <see cref="HardwareErrorClass.State"/>
        /// </summary>
        public static class State
        {
            /// <summary>
            /// Device (or driver) state cannot be changed (in this moment or for this configuration).
            /// </summary>
            public const ushort CannotChangeState = 0;
        }
    }
}
