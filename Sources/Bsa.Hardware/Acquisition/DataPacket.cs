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
using System.Text;
using System.Threading.Tasks;

namespace Bsa.Hardware.Acquisition
{
    /// <summary>
    /// Represents a packet containing samples acquired from an <see cref="AcquisitionDevice{T}"/>.
    /// </summary>
    [Serializable]
    public class DataPacket
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DataPacket"/>.
        /// </summary>
        /// <param name="driverId">
        /// The ID of the acquisition device driver (see <see cref="AcquisitionDevice.Id"/>) that acquired
        /// samples <paramref name="samples"/>. For virtual acquisition devices (for example concentrators)
        /// this ID may be <see cref="Guid.Empty"/>.
        /// </param>
        /// <param name="timestamp">
        /// Timestamp of the first acquired sample in this packet. If this information is not provided
        /// by hardware device then software driver must calculate this timestamp using a reference time
        /// taken when an acquisition session started (a session starts when acquisition mode changes from
        /// <see cref="AcquisitionMode.Idle"/> to anything else and it ends when mode goes back to
        /// <c>AcquisitionMode.Idle</c>) and an internal counter of received samples.
        /// During acquisition, for a sequence of packets, this time is a <strong>monotonic increasing</strong> clock.
        /// </param>
        /// <param name="samples">
        /// Acquired samples, first <em>dimension</em> (note it's not a multidimensional
        /// array but a jagged array) are channels and each array element is an array
        /// of samples acquired for that channel. This configuration allows you a fast access
        /// to channel's data and allows hardware to acquire in a multifrequency configuration
        /// (see <see cref="AcquisitionDeviceFeatures.Multifrequency"/>).
        /// Single elements may be <see langword="null"/> if no data has been acquired for that
        /// channel however not every consumer may support this <em>feature</em>.
        /// </param>
        public DataPacket(Guid driverId, DateTime timestamp, double[][] samples)
        {
            if (samples == null)
                throw new ArgumentNullException("samples");

            _acquisitionDeviceDriverId = driverId;
            _timestamp = timestamp;
            _samples = samples;
        }

        /// <summary>
        /// Gets the ID of the acquisition device driver that acquired these samples.
        /// </summary>
        /// <value>
        /// The ID of the acquisition device <see cref="AcquisitionDevice"/> that
        /// acquired this samples.
        /// </value>
        /// <seealso cref="AcquisitionDevice.Id"/>
        public Guid AcquisitionDeviceDriverId
        {
            get { return _acquisitionDeviceDriverId; }
        }

        /// <summary>
        /// Gets the timestamp of the first sample of <see cref="Samples"/>.
        /// </summary>
        /// <value>
        /// Timestamp of the first sample of <see cref="Samples"/>. During acquisition, for a sequence of packets,
        /// this time is a monotonic increasing clock.
        /// </value>
        public DateTime? Timestamp
        {
            get { return _timestamp; }
        }

        /// <summary>
        /// Gets the acquired samples.
        /// </summary>
        /// <value>
        /// Acquired samples, first <em>dimension</em> (note it's not a multidimensional
        /// array but a jagged array) are channels and each array element is an array
        /// of samples acquired for that channel. This configuration allows you a fast access
        /// to channel's data and allows hardware to acquire in a multifrequency configuration
        /// (see <see cref="AcquisitionDeviceFeatures.Multifrequency"/>).
        /// Single elements may be <see langword="null"/> if no data has been acquired for that
        /// channel however not every consumer may support this <em>feature</em>.
        /// </value>
        public double[][] Samples
        {
            get { return _samples; }
        }

        private readonly Guid _acquisitionDeviceDriverId;
        private readonly DateTime _timestamp;
        private readonly double[][] _samples;
    }
}
