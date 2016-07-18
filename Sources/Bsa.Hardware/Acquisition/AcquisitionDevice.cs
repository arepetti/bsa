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
        /// Feature that indicates if non-uniform sampling rate is supported.
        /// </summary>
        /// <remarks>
        /// <c>IsFeatureSamplingOnValueChangeAvailable</c> indicates whether feature is available for a specific hardware and
        /// <c>IsFeatureSamplingOnValueChangeEnabled</c> indicates if this feature is enabled for a particular configuration. If
        /// this feature is always enabled then <c>IsFeatureSamplingOnValueChangeEnabled</c> may be omitted. To support this feature
        /// may involve a lot of work for implementors because basic framework implementation does not offert any built-in support.
        /// </remarks>
        public static readonly DeviceFeature SamplingOnValueChange = new DeviceFeature(typeof(AcquisitionDevice), "SamplingOnValueChange");

        /// <summary>
        /// Feature that indicates is different channels may be acquired at different sampling rates.
        /// </summary>
        /// <remarks>
        /// <c>IsFeatureMultifrequencyAvailable</c> indicates whether feature is available for a specific hardware and
        /// <c>IsFeatureMultifrequencyEnabled</c> indicates if this feature is enabled for a particular configuration. If
        /// this feature is always enabled then <c>IsFeatureMultifrequencyEnabled</c> may be omitted.
        /// </remarks>
        public static readonly DeviceFeature Multifrequency = new DeviceFeature(typeof(AcquisitionDevice), "Multifrequency");

        /// <summary>
        /// Feature that indicates if device driver can update device's on-board firmware.
        /// </summary>
        /// <remarks>
        /// <c>IsFeatureFirmwareUpdateAvailable</c> indicates whether firmware update is available for a specific hardware and
        /// <c>IsFeatureFirmwareUpdateEnabled</c> indicates if firmware actually needs to be updated. Both functions must be
        /// specified, check is performed after connection has been estabilished. If connection has to be recreated after
        /// an update it's implementor's responsability to call <see cref="Device.Reconnect"/>.
        /// </remarks>
        public static readonly DeviceFeature FirmwareUpdate = new DeviceFeature(typeof(AcquisitionDevice), "FirmwareUpdate");

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
        public abstract void Setup();

        /// <summary>
        /// Validates device configuration (such as options and address).
        /// </summary>
        /// <exception cref="HardwareException">
        /// If device configuration is not valid.
        /// </exception>
        protected virtual void ValidateConfiguration()
        {
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();

            ValidateConfiguration();
        }

        protected override void OnConnected()
        {
            base.OnConnected();

            if (Features.IsAvailableAndEnabled(FirmwareUpdate))
                Features.Perform(FirmwareUpdate);
        }
    }

    /// <summary>
    /// Represents an acquisition device with a set of input physical channels from which data are acquired.
    /// </summary>
    /// <typeparam name="TChannelType">Effective type of channel descriptor.</typeparam>
    /// <remarks>
    /// Channel configuration is validated when <see cref="Setup"/> method is invoked (after device has been connected). To be used
    /// channel configuration must be sealed.
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
        /// <see cref="Setup"/> method is invoked.
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
        public override void Setup()
        {
            if (Status != DeviceConnectionStatus.Connected)
            {
                throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error,
                    HardwareErrorClass.State, HardwareErrorCodes.State.InvalidState, "Cannot setup device if it is not connected."));
            }

            if (!Channels.IsSealed)
            {
                throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error,
                    HardwareErrorClass.Arguments, HardwareErrorCodes.Arguments.InvalidChannelConfiguration, "Channel configuration must be sealed before you attempt to connect to device."));
            }

            ValidateChannelsConfiguration();
            SetupCore();
        }

        /// <summary>
        /// Setup hardware device with current configuration.
        /// </summary>
        protected abstract void SetupCore();

        /// <summary>
        /// Validates channels configuration (channels collection does not need to be sealed, this method may be invoked multiple times
        /// before acquisitor is frozen and <see cref="Setup"/> called).
        /// </summary>
        /// <exception cref="HardwareException">
        /// If channels configuration is not valid for this device.
        /// </exception>
        protected virtual void ValidateChannelsConfiguration()
        {
            if (Channels.Count == 0)
            {
                throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error,
                    HardwareErrorClass.Arguments, HardwareErrorCodes.Arguments.InvalidChannelConfiguration, "At least one channel must be specified."));
            }

            if (Channels.Distinct(x => x.SamplingRate).Count() > 1 && !Features.IsAvailable(Multifrequency))
            {
                throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error,
                    HardwareErrorClass.Arguments, HardwareErrorCodes.Arguments.InvalidChannelConfiguration, "Hardware does not support multifrequency acquisition, all channels must have same sampling rate."));
            }

            if (Channels.Any(x => x.SamplingRate == 0) && !Features.IsAvailable(SamplingOnValueChange))
            {
                throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error,
                    HardwareErrorClass.Arguments, HardwareErrorCodes.Arguments.InvalidChannelConfiguration, "Hardware does not support non uniform sampling, all channels must have a sampling rate higher than zero."));
            }

            if (!IsUniqueForChannels(x => x.Id))
            {
                throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error,
                    HardwareErrorClass.Arguments, HardwareErrorCodes.Arguments.InvalidChannelConfiguration, "All channels must have a different ID."));
            }

            if (!IsUniqueForChannels(x => x.Name))
            {
                throw new HardwareException(new HardwareError(HardwareErrorSeverity.Error,
                    HardwareErrorClass.Arguments, HardwareErrorCodes.Arguments.InvalidChannelConfiguration, "All channels must have a different name."));
            }
        }

        private readonly PhysicalChannelCollection<TChannelType> _channels;

        private bool IsUniqueForChannels<T>(Func<TChannelType, T> selector)
        {
            return Channels.Distinct(selector).Count() == Channels.Count;
        }
    }
}
