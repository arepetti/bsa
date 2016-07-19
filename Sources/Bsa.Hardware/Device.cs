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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Bsa.Instrumentation;

namespace Bsa.Hardware
{
    /// <summary>
    /// Base class for every hardware device.
    /// </summary>
    [DebuggerDisplay("{Address} [{Status}]")]
    public abstract class Device : Instrumentable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Device"/>.
        /// </summary>
        /// <param name="address">
        /// Address of the device to which connection will be estabilished.
        /// Format and content depends on the specific device.
        /// </param>
        protected Device(string address)
        {
            Address = address;
            _features = new FeatureCollection(this);
        }

        /// <summary>
        /// Gets the device address to which this driver must connect.
        /// </summary>
        /// <value>
        /// The device address to which this driver must connect in order to communicate with the device. Content
        /// and format of this property is implementation dependant and it may be a <see langword="null"/> or empty string
        /// if address is not applicable to this specific device.
        /// </value>
        public string Address
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current connection status to the device.
        /// </summary>
        /// <value>
        /// Connection status to the device. Almost all operations can be performed only when driver
        /// has been connected to the device with <see cref="Connect"/>. Initial value is <see cref="DeviceConnectionStatus.Disconnected"/>.
        /// </value>
        public DeviceConnectionStatus Status
        {
            get { return _status; }
            private set
            {
                if (_status != value)
                {
                    _status = value;
                    OnStatusChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets an object usable to check if a required feature is available and enabled.
        /// </summary>
        /// <value>
        /// An object which can be used to determine wheter a feature is available and/or enabled.
        /// </value>
        public FeatureCollection Features
        {
            get { return _features; }
        }

        /// <summary>
        /// Connects this driver to the device with address <see cref="Address"/>.
        /// </summary>
        /// <exception cref="HardwareException">
        /// If it's not possible to connect to device in this moment or if connection failed.
        /// </exception>
        public void Connect()
        {
            ThrowIfDisposed();

            if (Status.IsAnyOf(DeviceConnectionStatus.Connecting, DeviceConnectionStatus.Connected))
                return;
            
            if (!Status.IsAnyOf(DeviceConnectionStatus.Disconnected, DeviceConnectionStatus.Error))
                ThrowStateError(HardwareErrorCodes.State.CannotChangeState, String.Format("Cannot connect while in {0} state.", Status));

            Status = DeviceConnectionStatus.Connecting;
            OnConnecting();

            ReliabilityHelpers.ExecuteAndRetryOnError(
                () =>
                {
                    ConnectCore();
                },
                exception => // This error will be ignored and operation retryed
                {
                    HandleErrors(((HardwareException)exception).Errors);
                },
                exception => // This was the last attempt to perform this operation, exception will be re-thrown
                {
                    HandleErrors(((HardwareException)exception).Errors);
                    Status = DeviceConnectionStatus.Error;
                }
            );

            Telemetry.Increment(DeviceTelemetry.NumberOfSuccessfulConnections, 1);
            Status = DeviceConnectionStatus.Connected;
            OnConnected();
        }

        /// <summary>
        /// Disconnect this driver from a previously connected device.
        /// </summary>
        /// <exception cref="HardwareException">
        /// If it's not possible to perform disconnection in this moment or if disconnection failed.
        /// </exception>
        public void Disconnect()
        {
            ThrowIfDisposed();

            if (Status.IsAnyOf(DeviceConnectionStatus.Disconnecting, DeviceConnectionStatus.Disconnected))
                return;

            if (Status != DeviceConnectionStatus.Connected)
                ThrowStateError(HardwareErrorCodes.State.CannotChangeState, String.Format("Cannot disconnect while in {0} state.", Status));

            Status = DeviceConnectionStatus.Disconnecting;
            OnDisconnecting();

            try
            {
                DisconnectCore();
            }
            catch (HardwareException exception)
            {
                Telemetry.Increment(DeviceTelemetry.NumberOfFailedConnections, 1);
                HandleErrors(exception.Errors);

                throw;
            }

            // In case of error we do not go in "error state" but we consider device as disconnected (if exception
            // is handled elsewhere).
            Status = DeviceConnectionStatus.Disconnected;
            OnDisconnected();
        }

        /// <summary>
        /// Disconnect from acquisition device and connect again.
        /// </summary>
        /// <exception cref="HardwareException">
        /// If device is not actually connected.
        /// <br/>-or-<br/>
        /// If there has been an error during reconnection (in disconnection or connection phases).
        /// </exception>
        public void Reconnect()
        {
            if (Status != DeviceConnectionStatus.Connected)
                ThrowStateError(HardwareErrorCodes.State.CannotChangeState, String.Format("Cannot reconnect while in {0} state.", Status));

            Disconnect();
            Connect();
        }

        /// <summary>
        /// Event generated when <see cref="Status"/> property changes.
        /// </summary>
        public event EventHandler StatusChanged;

        /// <summary>
        /// In derived classes perform the connection to the device.
        /// </summary>
        /// <remarks>
        /// This method <strong>must</strong> wrap all possible exceptions into <see cref="HardwareException"/>.
        /// </remarks>
        protected abstract void ConnectCore();

        /// <summary>
        /// In derived classes perform the disconnection from the device.
        /// </summary>
        /// <remarks>
        /// This method <strong>must</strong> wrap all possible exceptions into <see cref="HardwareException"/>.
        /// </remarks>
        protected abstract void DisconnectCore();

        /// <summary>
        /// Function invoked just before the first attempt to connect to the device (<c>Status</c> is see <c>Connecting</c>).
        /// </summary>
        protected virtual void OnConnecting()
        {
        }

        /// <summary>
        /// Function invoked after a successful connection has been estabilished (<c>Status</c> is see <c>Connected</c>).
        /// </summary>
        protected virtual void OnConnected()
        {
        }

        /// <summary>
        /// Function invoked just before starting disconnection from the device (<c>Status</c> is see <c>Disconnecting</c>).
        /// </summary>
        protected virtual void OnDisconnecting()
        {
        }

        /// <summary>
        /// Function invoked after device has been disconnected without errors (<c>Status</c> is see <c>Disconnected</c>).
        /// </summary>
        protected virtual void OnDisconnected()
        {
        }

        /// <summary>
        /// Handles the specified set of errors.
        /// </summary>
        /// <param name="errors">List of errors occured when performing an operation.</param>
        protected virtual void HandleErrors(IEnumerable<HardwareError> errors)
        {
            Telemetry.Increment(DeviceTelemetry.NumberOfErrors, errors.Count());
        }

        /// <summary>
        /// Raises <see cref="StatusChanged"/> event.
        /// </summary>
        /// <param name="e">Additional event arguments.</param>
        protected virtual void OnStatusChanged(EventArgs e)
        {
            var statusChanged = StatusChanged;
            if (statusChanged != null)
                statusChanged(this, e);
        }

        /// <summary>
        /// Creates the object in charge to store telemetry data.
        /// </summary>
        /// <returns>
        /// A new <see cref="DeviceTelemetry"/> object, a derived class may disable instrumentation returning
        /// <c>NullTelemetryData(new DeviceTelemetry)</c>. See also <see cref="Instrumentable"/> class.
        /// </returns>
        protected override TelemetrySession CreateSession()
        {
            return new DeviceTelemetry();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                // If device is connected we disconnect it now, note that DeviceConnectionStatus.Connecting
                // is not aborted and device will be connected after this instance has been disposed. It's a logical
                // error and it should be avoided because it leaves the object in an indeterminate state.
                if (!IsDisposed && Status == DeviceConnectionStatus.Connected)
                    Disconnect();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Throws an exception of type <see cref="HardwareException"/> associated with an <see cref="HardwareError"/>
        /// with severity <see cref="HardwareErrorSeverity.Error"/>, class <see cref="HardwareErrorClass.State"/> and specified error code and error message.
        /// </summary>
        /// <param name="errorCode">The error code for this error.</param>
        /// <param name="message">The message that explains in detail this specific error.</param>
        protected static void ThrowStateError(ushort errorCode, string message)
        {
            throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error, HardwareErrorClass.State, errorCode, message));
        }

        /// <summary>
        /// Throws an exception of type <see cref="HardwareException"/> associated with an <see cref="HardwareError"/>
        /// with severity <see cref="HardwareErrorSeverity.Error"/>, class <see cref="HardwareErrorClass.Arguments"/> and specified error code and error message.
        /// </summary>
        /// <param name="errorCode">The error code for this error.</param>
        /// <param name="message">The message that explains in detail this specific error.</param>
        protected static void ThrowArgumentsError(ushort errorCode, string message)
        {
            throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error, HardwareErrorClass.Arguments, errorCode, message));
        }

        private DeviceConnectionStatus _status;
        private readonly FeatureCollection _features;
    }
}
