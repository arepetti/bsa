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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Bsa.Hardware
{
    /// <summary>
    /// Represents a feature available for a device.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public sealed class DeviceFeature : IEquatable<DeviceFeature>
    {
        /// <overload>
        /// Initializes a new instance of <see cref="DeviceFeature"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new instance of <see cref="DeviceFeature"/> associated with a specified device.
        /// </summary>
        /// <param name="associatedDevice">
        /// The type of the device to which this feature is associated. Type must be a class derived from <see cref="Device"/>.
        /// </param>
        /// <param name="name">
        /// Unique case-insensitive name of this feature. It must contain at least one alphanumeric character and feature name
        /// uniqueness is valiated only using US-ASCII alphanumeric characters. The name of the feature stripped from all non US-ASCII
        /// alphanumeric characters is called <em>equivalent name</em> and each feature must have an unique case-insensitive equivalent name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="associatedDevice"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="associatedDevice"/> is not a class derived from <see cref="Device"/>.
        /// <br/>-or-<br/>
        /// If <paramref name="name"/> is <see langword="null"/> or an empty string.
        /// <br/>-or-<br/>
        /// If <paramref name="name"/> is not a valid US-ASCII alphanumeric string.
        /// <br/>-or-<br/>
        /// If <paramref name="name"/> contains only non US-ASCII characters or it doesn't contain at least one alphanumeric character.
        /// </exception>
        public DeviceFeature(Type associatedDevice, string name)
        {
            if (associatedDevice == null)
                throw new ArgumentNullException("associatedDevice");

            if (!typeof(Device).IsAssignableFrom(associatedDevice))
                throw new ArgumentException("Associated device must be a class derived from Device.", "associatedDevice");

            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be a null or empty string.");

            AssociatedDevice = associatedDevice;
            Name = name.Trim();
            EquivalentName = StripNonAlphanumericCharacters(name);

            if (String.IsNullOrWhiteSpace(EquivalentName))
                throw new ArgumentException("Name cannot be an empty US-ASCII alphanumeric string.");
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DeviceFeature"/> associated with <see cref="Device"/>.
        /// </summary>
        /// <param name="name">
        /// Unique case-insensitive name of this feature. It must contain at least one alphanumeric character and feature name
        /// uniqueness is valiated only using US-ASCII alphanumeric characters. The name of the feature stripped from all non US-ASCII
        /// alphanumeric characters is called <em>equivalent name</em> and each feature must have an unique case-insensitive equivalent name.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <see langword="null"/> or an empty string.
        /// <br/>-or-<br/>
        /// If <paramref name="name"/> is not a valid US-ASCII alphanumeric string.
        /// <br/>-or-<br/>
        /// If <paramref name="name"/> contains only non US-ASCII characters or it doesn't contain at least one alphanumeric character.
        /// </exception>
        public DeviceFeature(string name)
            : this(typeof(Device), name)
        {
        }

        /// <summary>
        /// Gets the type of the device associated with this feature.
        /// </summary>
        /// <value>
        /// The type of the device associated with this feature; this feature can be checked only against specified
        /// device type (or one of its derived classes).
        /// </value>
        public Type AssociatedDevice
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of this feature.
        /// </summary>
        /// <value>
        /// The name of this feature as specified in constructor.
        /// </value>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The equivalent name of this feature.
        /// </summary>
        /// <value>
        /// Equivalent name of this feature. Each feature must have an unique equivalent name for an associated
        /// device. Equivalent name is <see cref="Name"/> stripped of all non US-ASCII alphanumeric characters.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string EquivalentName
        {
            get;
            private set;
        }

        public bool Equals(DeviceFeature other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;

            return String.Equals(this.Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DeviceFeature);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public static bool operator ==(DeviceFeature lhs, DeviceFeature rhs)
        {
            if (Object.ReferenceEquals(lhs, rhs))
                return true;

            if (Object.ReferenceEquals(lhs, null))
                return false;

            return lhs.Equals(rhs);
        }

        public static bool operator !=(DeviceFeature lhs, DeviceFeature rhs)
        {
            if (Object.ReferenceEquals(lhs, rhs))
                return false;

            if (Object.ReferenceEquals(lhs, null))
                return true;

            return !lhs.Equals(rhs);
        }

        private static string StripNonAlphanumericCharacters(string text)
        {
            // We can't simply enumerate System.Char because they're not code points but
            // code units (UTF-16). Converting to US-ASCII we strip all "unwanted" characters and
            // from them we keep only alphanumerics.
            try
            {
                var asciiText = Encoding.ASCII.GetChars(Encoding.ASCII.GetBytes(text));
                return new String(asciiText.Where(x => Char.IsLetterOrDigit(x)).ToArray());
            }
            catch (EncoderFallbackException exception)
            {
                throw new ArgumentException("Name is not a valid US-ASCII alphanumeric string.", exception);
            }
        }
    }
}
