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
using System.Runtime.Serialization;

namespace Bsa
{
    /// <summary>
    /// Represents an error specific of BSA framework.
    /// </summary>
    [Serializable]
    public class BsaException : Exception
    {
        /// <overload>
        /// Initializes a new instance of <see cref="BsaException"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new instance of <see cref="BsaException"/> with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BsaException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BsaException"/> with a specified error message and a reference to the
        /// inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">T
        /// The exception that is the cause of current exception of <see langword="null"/> if no inner exception is specified.
        /// </param>
        public BsaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BsaException"/> from serialized data.
        /// </summary>
        /// <param name="serializationInfo">Serialization data.</param>
        /// <param name="streamingContext">Serialization context.</param>
        protected BsaException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BsaException"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "This exception should not be used directly and in those rare cases a custom message should be provided.")]
        protected BsaException()
        {
        }
    }
}
