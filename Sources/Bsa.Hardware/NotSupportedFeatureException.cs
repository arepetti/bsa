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
using System.Linq;
using System.Runtime.Serialization;

namespace Bsa.Hardware
{
    /// <summary>
    /// Represents an error caused by the use of an unsupported hardware feature.
    /// </summary>
    [Serializable]
    public sealed class NotSupportedFeatureException : HardwareException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="NotSupportedFeatureException"/> containing the
        /// description of an errror caused by an unsupported hardware feature.
        /// </summary>
        /// <param name="errorCode">Error code (of class <see cref="HardwareErrorClass.Unsupported"/>) for this error.</param>
        /// <param name="message">The error message that describes the unsupported feature.</param>
        public NotSupportedFeatureException(ushort errorCode, string message)
            : base(new HardwareError(HardwareErrorSeverity.Error, HardwareErrorClass.Unsupported, errorCode, message))
        {
        }
    }
}
