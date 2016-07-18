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

namespace Bsa
{
    /// <summary>
    /// Basic implementation of <see cref="IDisposable"/> interface.
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        /// <summary>
        /// Releases the resources allocated by this object.
        /// </summary>
        ~Disposable()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases the resources allocated by this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Indicates whether this object has been disposed.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this object has been disposed (explictely with a call
        /// to <see cref="IDisposable.Dispose"/> or because it has been freed by Garbage Collector).
        /// </value>
        protected bool IsDisposed
        {
            get { return _isDisposed; }
        }

        /// <summary>
        /// Throws an exception if this object has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If this object has been disposed (<see cref="IsDisposed"/> is <see langword="true"/>).
        /// </exception>
        protected void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// Releases the resources allocated by this object.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> if resources should be released because this object has been explictely disposed
        /// or <see langword="false"/> if resources should be disposed because this object has been claimed by Garbage Collector.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            _isDisposed = true;
        }

        private bool _isDisposed;
    }
}
