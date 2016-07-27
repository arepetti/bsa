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
    /// Represents an error related to an hardware device.
    /// </summary>
    [Serializable]
    public class HardwareException : BsaException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HardwareException"/>.
        /// </summary>
        /// <summary>
        /// Initializes a new instance of <see cref="HardwareException"/> containing the specified list of hardware errors.
        /// </summary>
        /// <param name="errors">List of errors encapsulated inside this exception.</param>
        /// <param name="innerException">An optional exception that is the cause of all these errors or <see langword="null"/> if the root cause is not another exception.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="errors"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="errors"/> does not contain any element.
        /// <br/>-or-<br/>
        /// If any element of <paramref name="errors"/> is <see langword="null"/>.
        /// </exception>
        public HardwareException(IEnumerable<HardwareError> errors, Exception innerException = null)
            : base("Unknown hardware error.", innerException)
        {
            if (errors == null)
                throw new ArgumentNullException("errors");

            if (errors.Count() == 0)
                throw new ArgumentException("Cannot create an hardware exception without any hardware error.", "errors");

            if (errors.Any(x => x == null))
                throw new ArgumentException("List of errors cannot contain any null entry.");

            Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HardwareException"/> containing the hardware error.
        /// </summary>
        /// <param name="error">Error encapsulated inside this exception.</param>
        /// <param name="innerException">An optional exception that is the cause of this errors or <see langword="null"/> if the root cause is not another exception.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="error"/> is <see langword="null"/>.
        /// </exception>
        public HardwareException(HardwareError error, Exception innerException = null)
            : this(new HardwareError[] { error }, innerException)
        {
            if (error == null)
                throw new ArgumentNullException("error");
        }

        /// <summary>
        /// Gets the list of errors encapsulated by this exception.
        /// </summary>
        /// <value>
        /// The list of errors encapsulated by this exception. This enumeration will always have at least one element.
        /// </value>
        public IEnumerable<HardwareError> Errors
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error message from the first error of <see cref="Errors"/>.
        /// </summary>
        public override string Message
        {
            get
            {
                return Errors.First().Message;
            }
        }

        public override string ToString()
        {
            return String.Join(Environment.NewLine, Errors.Select(x => x.Message));
        }
    }
}
