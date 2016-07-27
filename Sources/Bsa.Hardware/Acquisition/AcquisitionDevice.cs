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
            // If required this code may be changed to convey more information: an "unique" part to identify each instance
            // and a fixed part that contains device driver information (like vendor and version). It may be necessary to change
            // its type to String (see also DataPacket.AcquisitionDeviceDriverId).
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets the ID of this acquisition device.
        /// </summary>
        /// <value>
        /// An ID that uniquely identify this acquisition device device driver. This ID is volatile, it means that it can't be used
        /// between different acquisition sessions and that should not be stored. This is is granted to be unique within
        /// a network then it can be used to identify multiple instances of the same driver (obvisously connected to different devices)
        /// within the same process, in multiple process and in a network of connected computers.
        /// </value>
        /// <seealso cref="HardwareId"/>
        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets/sets the ID of the hardware device.
        /// </summary>
        /// <value>
        /// After device has been connected with a call to <see cref="Device.Connect"/> it's possible to read its unique ID (if available).
        /// Content and format is implementation defined and it may not even be unique for each device. This value is <see langword="null"/>
        /// if this information is not available because device has not been connected or because this feature is not supported.
        /// Default value is <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// Do not confuse this ID with acquisition device driver ID <see cref="Id"/>, together (if <see cref="HardwareId"/> is available)
        /// they uniquely identify an acquisition device and connected device driver instance.
        /// </remarks>
        /// <seealso cref="Id"/>
        public string HardwareId
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets/sets the acquisition mode.
        /// </summary>
        /// <value>
        /// The acquisition mode for input data. Default value is <see cref="AcquisitionMode.Idle"/> and it should be
        /// changed to
        /// </value>
        /// <exception cref="NotSupportedFeatureException">
        /// If requested mode is not supported by hardware and cannot be software emulated.
        /// </exception>
        /// <exception cref="HardwareException">
        /// If hardware is not currently connected with <see cref="Device.Connect"/>.
        /// <br/>-or-<br/>
        /// If hardware has not been configured with <see cref="Setup"/>.
        /// </exception>
        public AcquisitionMode Mode
        {
            get { return _mode; }
            set
            {
                if (State != ConnectionState.Connected)
                    ThrowStateError(HardwareErrorCodes.State.InvalidState, "Cannot change acquisition mode if device is not connected.");

                if (!IsConfigured)
                    ThrowStateError(HardwareErrorCodes.State.InvalidState, "Cannot change acquisition mode if device has not been configured.");

                if (_mode != value)
                {
                    // I save old flag because calls to set_Mode may be nested (for example if an acquisition mode
                    // is temporary or a derived class decides to reject a specific mode). Note that event notification
                    // is inside then receivers may NOT be notified after they received some data of the new type (another
                    // reason calls may be nested).
                    bool oldCanOutputData = CanOutputData;

                    try
                    {
                        CanOutputData = false;

                        ChangeAcquisitionMode(value);
                        _mode = value;

                        OnModeChanged(EventArgs.Empty);
                    }
                    finally
                    {
                        CanOutputData = oldCanOutputData;
                    }
                }
            }
        }

        /// <summary>
        /// Setup hardware device with current configuration.
        /// </summary>
        /// <exception cref="HardwareException">
        /// Device setup is not valid or device cannot be configured in this moment.
        /// </exception>
        public virtual void Setup()
        {
            if (State != ConnectionState.Connected)
                ThrowStateError(HardwareErrorCodes.State.InvalidState, "Cannot setup device if it is not connected.");

            if (IsConfigured)
                ThrowStateError(HardwareErrorCodes.State.InvalidState, "Cannot setup device twice, if setup changed please first call Reconnect().");

            SetupCore();

            CanOutputData = true;
            IsConfigured = true;
            OnReady(EventArgs.Empty);
        }

        /// <summary>
        /// Event generated after device has been configured and it is ready to acquire data.
        /// </summary>
        public event EventHandler Ready;

        /// <summary>
        /// Event generated when acquisition mode property <see cref="Mode"/> changes.
        /// </summary>
        public event EventHandler ModeChanged;

        /// <summary>
        /// Events generated periodically when acquisition device is in <see cref="AcquisitionMode.Ohmeter"/> mode.
        /// </summary>
        /// <remarks>
        /// Different hardware devices may generate this event with different rules, it may be a complete list with
        /// all input channels (<see cref="AcquisitionDevice{T}.Channels"/>) where unknown/unavailable impedances have one
        /// or more <see langword="null"/> in <see cref="PhysicalChannelImpedance.Impedances"/> or a partial list
        /// which contains only channels where impedance is available/known.
        /// </remarks>
        public event EventHandler<OhmeterEventArgs> Ohmeter;

        /// <summary>
        /// Event generated when new acquired samples are available.
        /// </summary>
        public event EventHandler<DataEventArgs> Data;

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
        /// Indicates whether data acquired from hardware will be published by this driver.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if data acquired from hardware (if made available in current acquisition mode)
        /// are published by this driver.
        /// </value>
        protected bool CanOutputData
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
        /// Sets the acquisition mode.
        /// </summary>
        /// <param name="mode">The new required acquisition mode.</param>
        /// <remarks>
        /// Derived classes may override this function to customize how/when acquisition mode
        /// is changed, features emulation should be done with <see cref="Feature"/> specific syntax methods.
        /// When this method is called <see cref="AcquisitionDevice.Mode"/> still contains <em>old</em> mode.
        /// </remarks>
        protected virtual void ChangeAcquisitionMode(AcquisitionMode mode)
        {
            switch (mode)
            {
                case AcquisitionMode.Idle:
                    // Even if hardware does not support this we will "emulate" Idle disabling outputting, this
                    // is default behavior: derived classes may override this function if hardware support this.
                    break;
                case AcquisitionMode.Data:
                    // To (re)enable data acquisition we disable previously enabled "special" acquisition mode.
                    if (Mode == AcquisitionMode.Ohmeter)
                        Features.Perform(AcquisitionDeviceFeatures.Ohmeter);
                    else if (Mode == AcquisitionMode.Calibration)
                        Features.Perform(AcquisitionDeviceFeatures.Calibration);

                    // We do nothing here: it means that current mode is Idle and it's supported by hardware,
                    // it's derived classes task to enter/exit from idle mode (see case AcquisitionMode.Idle).
                    break;
                case AcquisitionMode.Ohmeter:
                    if (!Features.IsAvailable(AcquisitionDeviceFeatures.Ohmeter))
                        throw new NotSupportedFeatureException(HardwareErrorCodes.Unsupported.AcquisitionMode, "Inputs impedance check (ohmeter) is not supported.");

                    Features.Perform(AcquisitionDeviceFeatures.Ohmeter);
                    break;
                case AcquisitionMode.Calibration:
                    if (!Features.IsAvailable(AcquisitionDeviceFeatures.Calibration))
                        throw new NotSupportedFeatureException(HardwareErrorCodes.Unsupported.AcquisitionMode, "Inputs calibration signal is not supported.");

                    Features.Perform(AcquisitionDeviceFeatures.Calibration);
                    break;
                default:
                    throw new NotSupportedFeatureException(HardwareErrorCodes.Unsupported.AcquisitionMode, String.Format("Acquisition mode {0} is unknown.", mode));
            }
        }

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

        /// <summary>
        /// Generates event <see cref="ModeChanged"/>.
        /// </summary>
        /// <param name="e">Additional event arguments.</param>
        protected virtual void OnModeChanged(EventArgs e)
        {
            var modeChanged = ModeChanged;
            if (modeChanged != null)
                modeChanged(this, e);
        }

        /// <summary>
        /// Generates event <see cref="Ohmeter"/>.
        /// </summary>
        /// <param name="e">Additional arguments for this event.</param>
        protected virtual void OnOhmeter(OhmeterEventArgs e)
        {
            var ohmeter = Ohmeter;
            if (ohmeter != null)
                ohmeter(this, e);
        }

        /// <summary>
        /// Generates event <see cref="Data"/>.
        /// </summary>
        /// <param name="e">Additional arguments for this event.</param>
        /// <remarks>
        /// Event is not generated if <see cref="CanOutputData"/> is <see langword="false"/> (we're changing
        /// <see cref="Mode"/>) and when <see cref="Mode"/> is <see cref="AcquisitionMode.Idle"/> (data acquisition is paused).
        /// </remarks>
        protected virtual void OnData(DataEventArgs e)
        {
            // CanOutputData is false while we're changing acquisition mode then
            // we do not send samples out because consumers may have not received ModeChanged event yet.
            // We also do not produce data when we are in Idle mode, this emulates this feature when
            // hardware does not have a specific support for that.
            if (!CanOutputData || Mode == AcquisitionMode.Idle)
                return;

            var data = Data;
            if (data != null)
                data(this, e);
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

        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            _mode = AcquisitionMode.Idle;
        }

        private readonly Guid _id;
        private AcquisitionMode _mode;
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
