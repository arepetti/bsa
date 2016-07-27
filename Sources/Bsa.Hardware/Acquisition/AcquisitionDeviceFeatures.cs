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

namespace Bsa.Hardware.Acquisition
{
    /// <summary>
    /// Contains the definition of all the <see cref="AcquisitionDevice"/> known optional features.
    /// </summary>
    public static class AcquisitionDeviceFeatures
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
        public static readonly Feature SamplingOnValueChange = new Feature(typeof(AcquisitionDevice), "SamplingOnValueChange");

        /// <summary>
        /// Feature that indicates is different channels may be acquired at different sampling rates.
        /// </summary>
        /// <remarks>
        /// <c>IsFeatureMultifrequencyAvailable</c> indicates whether feature is available for a specific hardware and
        /// <c>IsFeatureMultifrequencyEnabled</c> indicates if this feature is enabled for a particular configuration. If
        /// this feature is always enabled then <c>IsFeatureMultifrequencyEnabled</c> may be omitted.
        /// </remarks>
        public static readonly Feature Multifrequency = new Feature(typeof(AcquisitionDevice), "Multifrequency");

        /// <summary>
        /// Feature that indicates if device driver can update device's on-board firmware.
        /// </summary>
        /// <remarks>
        /// <c>IsFeatureFirmwareUpdateAvailable</c> indicates whether firmware update is available for a specific hardware and
        /// <c>IsFeatureFirmwareUpdateEnabled</c> indicates if firmware actually needs to be updated. Both functions must be
        /// specified, check is performed after connection has been estabilished. If connection has to be recreated after
        /// an update it's implementor's responsability to call <see cref="Device.Reconnect"/>.
        /// </remarks>
        public static readonly Feature FirmwareUpdate = new Feature(typeof(AcquisitionDevice), "FirmwareUpdate");

        /// <summary>
        /// Feature that indicates if device supports impedance measurement of its inputs.
        /// </summary>
        /// <remarks>
        /// <c>IsFeatureOhmeterAvailable</c> indicates if this feature is supported, <c>IsFeatureOhmeterEnabled</c> indicates
        /// if this mode is currently active, use <c>PerformOhmeter</c> to enable this acquisition mode. Note that support
        /// is usually <em>static</em>, if this feature is not supported when device is in some specific states then it driver
        /// or hardware must throw <see cref="HardwareException"/> with the appropriate <see cref="HardwareError"/> (usually
        /// with class <see cref="HardwareErrorClass.State"/>).
        /// If free changing from one mode to the other is not supported (for example if device must always be changed to <em>idle</em>
        /// before switching mode between data/ohmeter/calibration) then it's device driver responsability to hide this detail: from
        /// caller point of view mode must be changed regardless current state.
        /// </remarks>
        public static readonly Feature Ohmeter = new Feature(typeof(AcquisitionDevice), "Ohmeter");

        /// <summary>
        /// Feature that indicates if device supports calibration of its inputs.
        /// </summary>
        /// <remarks>
        /// <c>IsFeatureCalibrationAvailable</c> indicates if this feature is supported, <c>IsFeatureCalibrationEnabled</c> indicates
        /// if this mode is currently active, use <c>PerformCalibration</c> to enable this acquisition mode. Note that support
        /// is usually <em>static</em>, if this feature is not supported when device is in some specific states then it driver
        /// or hardware must throw <see cref="HardwareException"/> with the appropriate <see cref="HardwareError"/> (usually
        /// with class <see cref="HardwareErrorClass.State"/>).
        /// If free changing from one mode to the other is not supported (for example if device must always be changed to <em>idle</em>
        /// before switching mode between data/ohmeter/calibration) then it's device driver responsability to hide this detail: from
        /// caller point of view mode must be changed regardless current state.
        /// </remarks>
        public static readonly Feature Calibration = new Feature(typeof(AcquisitionDevice), "Calibration");

        /// <summary>
        /// Feature that indicates if device can pull-down its inputs to 0.
        /// </summary>
        /// <remarks>
        /// <c>IsFeaturePullInputsDownAvailable</c> indicates if this feature is supported, <c>IsFeaturePullInputsDownEnabled</c> indicates
        /// if this mode is currently active, use <c>PerformPullInputsDown</c> to perform this operation. Note that this feature is usually
        /// supported in data acquisition mode, driver or hardware must throw <see cref="HardwareException"/> with the appropriate <see cref="HardwareError"/>
        /// (usually with class <see cref="HardwareErrorClass.State"/>). Optional parameter of <c>PeformPullInputsDown</c> may be used to
        /// specify a duration, <c>Object</c> must be casted to <c>TimeSpan</c> with two special values: <c>TimeSpan.MaxValue</c> to permanently
        /// enable this mode until this function is called with <c>TimeSpan.Zero</c> to disable it. Calling this method with <c>TimeSpan.Zero</c>
        /// when this mode is disabled doesn't perform anything (and returns current state). If parameter is <see langword="null"/> then
        /// default behavior is performed (device dependant). If device doesn't fully support this feature then drivers must emulate required behavior.
        /// </remarks>
        public static readonly Feature PullInputsDown = new Feature(typeof(AcquisitionDevice), "PullInputsDown");

    }
}
