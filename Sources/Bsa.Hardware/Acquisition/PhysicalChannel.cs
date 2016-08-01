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
    /// Represents a physical channel as acquired by an <see cref="AcquisitionDevice"/>.
    /// </summary>
    [Serializable]
    public class PhysicalChannel : Sealable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PhysicalChannel"/> with specified ID.
        /// </summary>
        /// <param name="id">Unique ID of this physical channel.</param>
        public PhysicalChannel(Guid id)
        {
            _id = id;
            _range = new Range<double>(-1, 1);
        }

        /// <summary>
        /// Gets the ID of this channel.
        /// </summary>
        /// <value>
        /// The unique ID of this channel. ID format and content is completely implementation defined
        /// and it may even be a <em>random</em> ID, only constraint is to be unique within the same
        /// <see cref="PhysicalChannelCollection{T}"/>.
        /// </value>
        /// <seealso cref="Name"/>
        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets/sets the name of this channel.
        /// </summary>
        /// <value>
        /// The display name of this channel. Display name is used to identify this channel using a friendly name,
        /// there aren't restrictions on its content but it has to be unique within the same <see cref="PhysicalChannelCollection{T}"/>.
        /// <c>Name</c> is not directly related to <see cref="Id"/> but they represent the same thing from two points of view:
        /// name is how all the other components will refer to this channel and ID is how hardware (and hardware only) will refer to
        /// this channel, this class is the junction point between these two views.
        /// </value>
        /// <exception cref="ArgumentException">
        /// If <see langword="value"/> is a <see langword="null"/> or blank string.
        /// </exception>
        /// <seealso cref="Id"/>
        public string Name
        {
            get { return _name; }
            set
            {
                ThrowIfSealed();

                if (String.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Channel name cannot be a null or blank string.");

                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the sampling rate (in Hertz) for this channel.
        /// </summary>
        /// <value>
        /// The sampling rate in Hertz for this channel. 0 means <em>sampled on change</em> if it's supported
        /// (see <see cref="AcquisitionDeviceFeatures.SamplingOnValueChange"/> feature) otherwise it a placeholder for an invalid value.
        /// Usage of this property is implementation defined. If hardware supports acquisition of channels with multiple frequencies
        /// (see <see cref="AcquisitionDeviceFeatures.Multifrequency"/> feature) then this value may be different for channels of the same set
        /// otherwise it must be the same for all channels. Default value is 0.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If value is less than zero.
        /// </exception>
        /// <seealso cref="Id"/>
        /// <remarks>
        /// Even if sampling <em>rate</em> is technically the incorrect word for this value I found that it
        /// is in such common use that moving to sampling <em>frequency</em> or sampling <em>period</em>
        /// would be even more confusing. Don't forget that (unless we introduce a <c>Quantity</c> object
        /// which embeds its unit of measure) this value is always expressed in Hertz.
        /// </remarks>
        public double SamplingRate
        {
            get { return _samplingRate; }
            set
            {
                ThrowIfSealed();

                if (value < 0)
                    throw new ArgumentOutOfRangeException("Sampling rate cannot be a negative value");

                _samplingRate = value;
            }
        }

        /// <summary>
        /// Gets/sets the physical range of this input.
        /// </summary>
        /// <value>
        /// The physical range of this input. Not all hardware devices support this feature, drivers
        /// may throw an exception if range selection is not available (or given value is out of range).
        /// Unit of measure is implementation defined. Default value is [-1 +1].
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <c>Minimum</c> is higher or equal than <c>Maximum</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If minimum or maximum value is <see cref="Double.NegativeInfinity"/>, <see cref="Double.PositiveInfinity"/>
        /// or <see cref="Double.NaN"/>.
        /// </exception>
        public Range<double> Range
        {
            get { return _range; }
            set
            {
                ThrowIfSealed();

                if (value.Minimum > value.Maximum)
                    throw new ArgumentOutOfRangeException("Minimum cannot be higher than maximum.");

                if (value.Minimum == value.Maximum)
                    throw new ArgumentOutOfRangeException("Minimum and maximum value must be different.");

                if (Double.IsInfinity(value.Minimum) || Double.IsNaN(value.Minimum))
                    throw new ArgumentException("Minimum value cannot be Infinity or NaN.");

                if (Double.IsInfinity(value.Maximum) || Double.IsNaN(value.Maximum))
                    throw new ArgumentException("Maximum value cannot be Infinity or NaN.");

                _range = value;
            }
        }

        protected override Sealable CreateNewInstance()
        {
            return new PhysicalChannel(Id);
        }

        protected override void CopyPropertiesTo(Sealable target)
        {
            PhysicalChannel other = (PhysicalChannel)target;
            other._name = this._name;
            other._samplingRate = this._samplingRate;
            other._range = this._range;
        }

        private readonly Guid _id;
        private string _name;
        private double _samplingRate;
        private Range<double> _range;
    }
}
