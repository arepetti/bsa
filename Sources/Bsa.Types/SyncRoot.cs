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
using System.Threading;

namespace Bsa
{
    /// <summary>
    /// Represents an easy to use reader/writer lock mechanism.
    /// </summary>
    /// <remarks>
    /// Current implementation is tied to <see cref="ReaderWriterLockSlim"/> but using this class allows you
    /// to change this implementation detail in future (keeping the same reader/writer lock semantic). Syntax also
    /// allows you to simply enter/exit locks using familiar <see langword="using"/> statement:
    /// <code>
    /// using (var writeLock = _syncRoot.EnterWriteLock())
    /// {
    ///     /* your code */
    /// }
    /// </code>
    /// Instead of something similar to this:
    /// <code>
    /// _readWriterLock.EnterWriteLock();
    /// try
    /// {
    ///     /* your code */
    /// }
    /// finally
    /// {
    ///     _readWriterLock.ExitWriteLock();
    /// }
    /// </code>
    /// </remarks>
    public sealed class SyncRoot : IDisposable
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReaderWriterLockSlim"/>
        /// </summary>
        /// <param name="lockRecursionPolicy">Indicate wheter a lock can be entered multiple times by the same thread.</param>
        public SyncRoot(LockRecursionPolicy lockRecursionPolicy = LockRecursionPolicy.NoRecursion)
        {
            _lock = new ReaderWriterLockSlim(lockRecursionPolicy);
        }

        /// <summary>
        /// Release resources acquired by this object.
        /// </summary>
        ~SyncRoot()
        {
            Dispose(false);
        }

        /// <summary>
        /// Tries to enter the lock in read mode.
        /// </summary>
        /// <returns>
        /// An opaque <see cref="IDisposable"/> object that will release acquired lock when <see cref="IDisposable.Dispose"/>
        /// will be invoked. Note that manually invoking <see cref="IDisposable.Dispose"/> multiple times on this object may throw
        /// <see cref="SynchronizationLockException"/>.
        /// </returns>
        /// <exception cref="LockRecursionException">
        /// If lock has been created with <see cref="LockRecursionPolicy.NoRecursion"/> but this
        /// function has been called again from the same thread without first releasing previous lock.
        /// </exception>
        public IDisposable EnterReadLock()
        {
            _lock.EnterReadLock();
            return new Disposable(() => _lock.ExitReadLock());
        }

        /// <summary>
        /// Tries to enter the lock in read mode (but with the option to upgrade to write mode in future).
        /// </summary>
        /// <returns>
        /// An opaque <see cref="IDisposable"/> object that will release acquired lock when <see cref="IDisposable.Dispose"/>
        /// will be invoked. Note that manually invoking <see cref="IDisposable.Dispose"/> multiple times on this object may throw
        /// <see cref="SynchronizationLockException"/>.
        /// </returns>
        /// <exception cref="LockRecursionException">
        /// If lock has been created with <see cref="LockRecursionPolicy.NoRecursion"/> but this
        /// function has been called again from the same thread without first releasing previous lock.
        /// </exception>
        public IDisposable EnterUpgradeableReadLock()
        {
            _lock.EnterUpgradeableReadLock();
            return new Disposable(() => _lock.ExitUpgradeableReadLock());
        }

        /// <summary>
        /// Tries to enter the lock in write mode.
        /// </summary>
        /// <returns>
        /// An opaque <see cref="IDisposable"/> object that will release acquired lock when <see cref="IDisposable.Dispose"/>
        /// will be invoked. Note that manually invoking <see cref="IDisposable.Dispose"/> multiple times on this object may throw
        /// <see cref="SynchronizationLockException"/>.
        /// </returns>
        /// <exception cref="LockRecursionException">
        /// If lock has been created with <see cref="LockRecursionPolicy.NoRecursion"/> but this
        /// function has been called again from the same thread without first releasing previous lock.
        /// </exception>
        public IDisposable EnterWriteLock()
        {
            _lock.EnterWriteLock();
            return new Disposable(() => _lock.ExitWriteLock());
        }

        /// <summary>
        /// Releases resources acquired by this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private sealed class Disposable : IDisposable
        {
            public Disposable(Action onDisposing)
            {
                _onDisposing = onDisposing;
            }

            public void Dispose()
            {
                _onDisposing();
            }

            private readonly Action _onDisposing;
        }

        private readonly ReaderWriterLockSlim _lock;

        private void Dispose(bool disposing)
        {
            if (disposing)
                _lock.Dispose();
        }
    }
}
