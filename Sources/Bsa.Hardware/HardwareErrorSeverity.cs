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
    /// Specifies the severity of an hardware error.
    /// </summary>
    public enum HardwareErrorSeverity
    {
        /// <summary>
        /// Error is really just a warning, an action may not be completed but it does not prevent normal operations.
        /// </summary>
        Warning = 0,

        /// <summary>
        /// Error prevents a required action to be completed. Device is still operation and other actions may be executed.
        /// </summary>
        Error = 1,

        /// <summary>
        /// Error prevents a required action to be completed and device may not be operational, it may need a reset to restore
        /// normal functions or it may be permanently unaccessible.
        /// </summary>
        Critical = 2,
    }
}
