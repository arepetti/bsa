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
using System.ComponentModel;

namespace Bsa.Hardware
{
    /// <summary>
    /// Represents an hardware error.
    /// </summary>
    [Serializable]
    public sealed class HardwareError
    {
        /// <overload>
        /// Initializes a new instance of <see cref="Hardware"/> error.
        /// </overload>
        /// <summary>
        /// Initializes a new instance of <see cref="Hardware"/> error.
        /// </summary>
        /// <param name="severity">Severity of this error.</param>
        /// <param name="errorClass">The class for this error, in practice a category to group the error.</param>
        /// <param name="errorCode">
        /// The code that uniquely identify this error. Each error code must be unique within the same error class.
        /// </param>
        /// <param name="message">Descriptive (non localized) message of this error.</param>
        /// <exception cref="InvalidEnumArgumentException">
        /// If <paramref name="severity"/> is not a valid <see cref="HardwareErrorSeverity"/> value.
        /// <br/>-or-<br/>
        /// If <paramref name="errorClass"/> is not a valid <see cref="HardwareErrorClass"/> value.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if <paramref name="message"/> is a null or blank string.
        /// </exception>
        public HardwareError(HardwareErrorSeverity severity, HardwareErrorClass errorClass, ushort errorCode, string message)
        {
            if (!Enum.IsDefined(typeof(HardwareErrorClass), errorClass))
                throw new InvalidEnumArgumentException("errorClass", (int)errorClass, typeof(HardwareErrorClass));

            if (!Enum.IsDefined(typeof(HardwareErrorSeverity), severity))
                throw new InvalidEnumArgumentException("severity", (int)severity, typeof(HardwareErrorSeverity));

            if (String.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Error message cannot be a null or blank string.", "message");

            Severity = severity;
            Class = errorClass;
            Code = errorCode;
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Hardware"/> error to represent a generic unspecified error (with
        /// class <see cref="HardwareErrorClass.Generic"/> and error code 0).
        /// </summary>
        /// <param name="severity">Severity of this error.</param>
        /// <param name="message">Descriptive (non localized) message of this error.</param>
        /// <exception cref="InvalidEnumArgumentException">
        /// If <paramref name="severity"/> is not a valid <see cref="HardwareErrorSeverity"/> value.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// if <paramref name="message"/> is a null or blank string.
        /// </exception>
        public HardwareError(HardwareErrorSeverity severity, string message)
            : this(severity, HardwareErrorClass.Generic, HardwareErrorCodes.Generic.UnknownError, message)
        {
        }

        /// <summary>
        /// Gets the severity of this error.
        /// </summary>
        /// <value>
        /// The severity of this error, indicates if error might be ignored (warning) or a corrective action
        /// should be taken (and if device is still operational).
        /// </value>
        public HardwareErrorSeverity Severity
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the class of this error.
        /// </summary>
        /// <value>
        /// The class of this error as defined in <see cref="HardwareErrorClass"/>. Some classes denote temporary
        /// errors which may be <em>automatically</em> recovered after a short time, use <see cref="IsRetryable"/> to
        /// determine if errors of this class may be transient and operation may be retryed after an arbitrary delay.
        /// </value>
        public HardwareErrorClass Class
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the numeric code of this error.
        /// </summary>
        /// <value>
        /// The numeric code of this error, unique within the same class (see <see cref="Class"/>).
        /// </value>
        public ushort Code
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a descriptive and non localized message about this error.
        /// </summary>
        /// <value>
        /// Descriptive message about this error. It may contain any additional information driver
        /// developer considers useful to diagnose this error. This error message is not localized to UI culture and
        /// it should always be in English to facilitate debugging and error reporting regardless application UI culture.
        /// </value>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines if error represented by this object is a temporary error which may be <em>automatically</em>
        /// recovered waiting an arbitrary amount of time.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this object represents a temporary error and operation may be retryed after
        /// an arbitrary amount of time. Note that this function is based only on error class then not all errors
        /// within the same class may be temporary and errors of other classes may be temporary too.
        /// </returns>
        public bool IsRetryable()
        {
            return Class == HardwareErrorClass.Generic || Class == HardwareErrorClass.Communication;
        }

        /// <summary>
        /// Converts this hardware error object into an <c>HRESULT</c> code.
        /// </summary>
        /// <returns>
        /// An <c>HRESULT</c> code for this error where <see cref="Class"/> is the <em>facility</em>
        /// and <see cref="Code"/> is the <em>code</em>. Note that both <em>Severity</em> and <em>Customer</em>
        /// bits are set and all reserved bits are unset.
        /// </returns>
        public uint ToHResult()
        {
            return 0xA0000000 | ((((uint)Class) & 0x7FF) << 16) | Code;
        }

        public override string ToString()
        {
            return String.Format("[{0}] {1} ({2})", Severity, Message, ToHResult());
        }
    }
}
