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

namespace Bsa.Hardware.Acquisition
{
    /// <summary>
    /// Represents a generic acquisition device for analogic signals.
    /// </summary>
    public abstract class AcquisitionDevice : Device
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AcquisitionDevice"/> with specified address.
        /// </summary>
        /// <param name="address">
        /// Address of the device to which connection will be estabilished.
        /// Format and content depends on the specific device.
        /// </param>
        protected AcquisitionDevice(string address)
            : base(address)
        {
        }

        /// <summary>
        /// Setup hardware device with current configuration.
        /// </summary>
        /// <exception cref="HardwareException">
        /// Device setup is not valid or device cannot be configured in this moment.
        /// </exception>
        public virtual void Setup()
        {
            if (Status != DeviceConnectionStatus.Connected)
                ThrowStateError(HardwareErrorCodes.State.InvalidState, "Cannot setup device if it is not connected.");

            if (IsConfigured)
                ThrowStateError(HardwareErrorCodes.State.InvalidState, "Cannot setup device twice, if setup changed please first call Reconnect().");

            SetupCore();

            IsConfigured = true;
            OnReady(EventArgs.Empty);
        }

        /// <summary>
        /// Events generated after device has been configured and it is ready to acquire data.
        /// </summary>
        public event EventHandler Ready;

        /// <summary>
        /// Indicates whether device has been configured (calling <see cref="Setup"/>) and it is ready for acquisition.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if device has been configured (with a call to <see cref="Setup"/>) and it is now ready for acquisition. If setup
        /// failed with an exception this property may be <see langword="true"/> but device might be unusable. Disconnection and reconnection reset this
        /// property to default <see langword="false"/> value.
        /// </value>
        protected bool IsConfigured
        {
            get;
            private set;
        }

        /// <summary>
        /// Validates device configuration (such as options and address).
        /// </summary>
        /// <exception cref="HardwareException">
        /// If device configuration is not valid.
        /// </exception>
        protected virtual void ValidateConfiguration()
        {
        }

        /// <summary>
        /// Setup hardware device with current configuration.
        /// </summary>
        protected abstract void SetupCore();

        /// <summary>
        /// Generates event <see cref="Ready"/>.
        /// </summary>
        /// <param name="e">Additional event arguments.</param>
        protected virtual void OnReady(EventArgs e)
        {
            var ready = Ready;
            if (ready != null)
                ready(this, e);
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            ValidateConfiguration();
        }

        protected override void OnConnected()
        {
            base.OnConnected();

            if (Features.IsAvailableAndEnabled(AcquisitionDeviceFeatures.FirmwareUpdate))
                Features.Perform(AcquisitionDeviceFeatures.FirmwareUpdate);
        }

        protected override void OnDisconnecting()
        {
            base.OnDisconnecting();

            IsConfigured = false;
        }
    }

    /// <summary>
    /// Represents an acquisition device with a set of input physical channels from which data are acquired.
    /// </summary>
    /// <typeparam name="TChannelType">Effective type of channel descriptor.</typeparam>
    /// <remarks>
    /// Channel configuration is validated when <see cref="AcquisitionDevice.Setup"/> method is invoked (after device has been connected).
    /// To be used channel configuration must be sealed.
    /// </remarks>
    public abstract class AcquisitionDevice<TChannelType> : AcquisitionDevice
        where TChannelType : PhysicalChannel
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AcquisitionDevice"/> with specified address.
        /// </summary>
        /// <param name="address">
        /// Address of the device to which connection will be estabilished.
        /// Format and content depends on the specific device.
        /// </param>
        protected AcquisitionDevice(string address)
            : base(address)
        {
            _channels = new PhysicalChannelCollection<TChannelType>();
        }

        /// <summary>
        /// Gets a reference the list of physical channels that must be acquired.
        /// </summary>
        /// <value>
        /// The list of physical channels that must be acquired, this collection must be sealed before
        /// <see cref="AcquisitionDevice.Setup"/> method is invoked.
        /// </value>
        public PhysicalChannelCollection<TChannelType> Channels
        {
            get { return _channels; }
        }

        /// <summary>
        /// Setup hardware device with current configuration.
        /// </summary>
        /// <exception cref="HardwareException">
        /// If channel configuration is not valid.
        /// <br/>-or-<br/>
        /// If channel collection <see cref="Channels"/> has not been sealed (calling <c>Channels.Seal()</c>).
        /// </exception>
        protected override void SetupCore()
        {
            if (!Channels.IsSealed)
                ThrowArgumentsError(HardwareErrorCodes.Arguments.InvalidChannelConfiguration, "Channel configuration must be sealed before you attempt to connect to device.");

            ValidateChannelsConfiguration();
        }

        /// <summary>
        /// Validates channels configuration (channels collection does not need to be sealed, this method may be invoked multiple times
        /// before acquisitor is frozen and <see cref="AcquisitionDevice.Setup"/> called).
        /// </summary>
        /// <exception cref="HardwareException">
        /// If channels configuration is not valid for this device.
        /// </exception>
        protected virtual void ValidateChannelsConfiguration()
        {
            if (Channels.Count == 0)
                ThrowArgumentsError(HardwareErrorCodes.Arguments.InvalidChannelConfiguration, "At least one channel must be specified.");

            if (Channels.Distinct(x => x.SamplingRate).Count() > 1 && !Features.IsAvailable(AcquisitionDeviceFeatures.Multifrequency))
                ThrowArgumentsError(HardwareErrorCodes.Arguments.InvalidChannelConfiguration, "Hardware does not support multifrequency acquisition, all channels must have same sampling rate.");

            if (Channels.Any(x => x.SamplingRate == 0) && !Features.IsAvailable(AcquisitionDeviceFeatures.SamplingOnValueChange))
                ThrowArgumentsError(HardwareErrorCodes.Arguments.InvalidChannelConfiguration, "Hardware does not support non uniform sampling, all channels must have a sampling rate higher than zero.");

            if (!IsUniqueForChannels(x => x.Id))
                ThrowArgumentsError(HardwareErrorCodes.Arguments.InvalidChannelConfiguration, "All channels must have a different ID.");

            if (!IsUniqueForChannels(x => x.Name))
                ThrowArgumentsError(HardwareErrorCodes.Arguments.InvalidChannelConfiguration, "All channels must have a different name.");
        }

        private readonly PhysicalChannelCollection<TChannelType> _channels;

        private bool IsUniqueForChannels<T>(Func<TChannelType, T> selector)
        {
            return Channels.Distinct(selector).Count() == Channels.Count;
        }
    }
}
