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

namespace Bsa.Instrumentation
{
    /// <summary>
    /// Base class of objects with support for application telemetry.
    /// </summary>
    public abstract class Instrumentable : Disposable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Instrumentable"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "NullTelemetrySession", Justification = "It is a class name.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This is the intended behavior.")]
        protected Instrumentable()
        {
            // TODO: we may introduce a call to a virtual method (then overrideable in derived classes) to disable instrumentation (current workaround is wrapping
            // returned object into a NullTelemetrySession object).
            _telemetry = CreateSession();
            if (_telemetry == null)
                throw new InvalidOperationException("An instrumentable object must have a valid telemetry session, use NullTelemetrySession if you do not need it.");

            _telemetry.Start();
        }

        /// <summary>
        /// Gets the object in charge of recording telemetry.
        /// </summary>
        /// <value>
        /// The object, used by derived classes, that exposes methods to record telemetry data.
        /// </value>
        /// <remarks>
        /// This object must be a class derived from <see cref="WpcTelemetrySession"/> returned by <see cref="CreateSession"/> method,
        /// see class definition for details about telemetry data declaration. Telemetry is <em>started</em> when this object is created and stopped
        /// when it's disposed (directly or by Garbage Collector).
        /// </remarks>
        protected TelemetrySession Telemetry
        {
            get { return _telemetry; }
        }

        /// <summary>
        /// Creates a new object, used by derived classes, that exposed methods to record telemetry data.
        /// </summary>
        /// <returns>
        /// A new instance of a class derived from <see cref="TelemetrySession"/>, derived classes can use <see cref="Telemetry"/> property
        /// to access to this object later. This method should not be invoked directly by derived classes.
        /// </returns>
        protected abstract TelemetrySession CreateSession();

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && Telemetry != null)
                    ((IDisposable)Telemetry).Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private readonly TelemetrySession _telemetry;
    }
}
