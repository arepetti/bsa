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
            public const ushort Unknown = 0;

            /// <summary>
            /// An hardware of software limit has been reached.
            /// </summary>
            public const ushort LimitReached = 1;

            /// <summary>
            /// Timeout error, an operation cannot be completed within the maximum allowed time.
            /// </summary>
            public const ushort Timeout = 2;
        }

        /// <summary>
        /// Containst constants for well-known internal errors.
        /// </summary>
        public static class Internal
        {
            /// <summary>
            /// Generic error (and there is not a more specific error code).
            /// </summary>
            public const ushort Generic = 0;

            /// <summary>
            /// Driver performed an invalid or unexpected operation, this error is often caused by a programming error.
            /// </summary>
            public const ushort InvalidOperation = 1;
        }

        /// <summary>
        /// Containst constants for well-known errors in drivers arguments/setup.
        /// </summary>
        public static class Arguments
        {
            /// <summary>
            /// Generic error (and there is not a more specific error code).
            /// </summary>
            public const ushort Generic = 0;

            /// <summary>
            /// Supplied arguments or configuration for channels is not valid.
            /// </summary>
            public const ushort InvalidChannelConfiguration = 1;
        }

        /// <summary>
        /// Contains constants for well-known hardware errors for class <see cref="HardwareErrorClass.State"/>
        /// </summary>
        public static class State
        {
            /// <summary>
            /// Generic error (and there is not a more specific error code).
            /// </summary>
            public const ushort Generic = 0;

            /// <summary>
            /// Device (or driver) state cannot be changed (in this moment or for this configuration).
            /// </summary>
            public const ushort CannotChangeState = 1;

            /// <summary>
            /// An operation cannot be performed while driver/device is in the current state.
            /// </summary>
            public const ushort InvalidState = 2;
        }

        /// <summary>
        /// Contains constants for well-known hardware errors for class <see cref="HardwareErrorClass.Unsupported"/>
        /// </summary>
        public static class Unsupported
        {
            /// <summary>
            /// Generic error (and there is not a more specific error code).
            /// </summary>
            public const ushort Generic = 0;

            /// <summary>
            /// A required generic feature is not supported by hardware and (currently) cannot be software emulated.
            /// </summary>
            public const ushort CannotBeEmulated = 1;

            /// <summary>
            /// Required acquisition mode is not supported by hardware and cannot be software emulated.
            /// </summary>
            public const ushort AcquisitionMode = 2;
        }
    }
}
