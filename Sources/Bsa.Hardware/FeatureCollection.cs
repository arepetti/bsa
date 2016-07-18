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

namespace Bsa.Hardware
{
    /// <summary>
    /// Exposes methods to determine if a device feature is available and enabled.
    /// </summary>
    /// <remarks>
    /// Derived classes of <see cref="Device"/> can expose methods to determine if their features
    /// are available and enabled using methods with conventional names. Methods can be instance (virtual or not)
    /// or static methods but they must respect following rules:
    /// <list type="bullet">
    /// <item>Method cannot have parameters.</item>
    /// <item>Method must return <see cref="Boolean"/>.</item>
    /// <item>
    /// Method name must have prefix <c>IsFeature</c> followed by feature's equivalent name and then a suffix to indicate
    /// if method is used to determine whether a feature is available (<c>IsFeature*Available</c>) or enabled (<c>IsFeature*Enabled</c>).
    /// Name matching is case insensitive.
    /// </item>
    /// <item>
    /// Derived classes may override base class method if it is virtual but overriding hiding base method signature is not supported.
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class FeatureCollection
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FeatureCollection"/>.
        /// </summary>
        /// <param name="device">The device to which this feature-set is associated.</param>
        internal FeatureCollection(Device device)
        {
            _methodsToCheckIfAvailable = new DeviceFeatureMethodDictionary(device, "IsFeature{0}Available");
            _methodsToCheckIfEnabled = new DeviceFeatureMethodDictionary(device, "IsFeature{0}Enabled");
        }

        /// <summary>
        /// Determines whether specified feature is available.
        /// </summary>
        /// <param name="feature">The feature you want to check.</param>
        /// <returns>
        /// <see langword="true"/> if specified feature is available. If device doesn't specify its support for
        /// this feature then it assumes feature is not available.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If  <paramref name="feature"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If specified feature is not associated with device linked with this object (or to one of its derived classes).
        /// </exception>
        public bool IsAvailable(DeviceFeature feature)
        {
            if (feature == null)
                throw new ArgumentNullException("feature");

            return _methodsToCheckIfAvailable.Read(feature, () => false);
        }

        /// <summary>
        /// Determines whether specified feature is enabled.
        /// </summary>
        /// <param name="feature">The feature you want to check.</param>
        /// <returns>
        /// <see langword="true"/> if specified feature is enabled. If device doesn't specify any method
        /// to determine if a feature is enabled then it assumes it's enabled if it is available.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If  <paramref name="feature"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If specified feature is not associated with device linked with this object (or to one of its derived classes).
        /// </exception>
        /// <remarks>
        /// Note that it's caller responsability to determine if a feature is available before checking if it
        /// is enabled. This method check for availability (as default for this method) only if device does not provide
        /// a method to determine it. If, for example, an hypotetic device has a method <c>bool IsFeatureXyzEnabled() { return true; }</c>
        /// but it has not a complementary method <c>IsFeatureXyzAvailable</c> then, if caller does not check first for availability,
        /// application behavior is undefined.
        /// </remarks>
        public bool IsEnabled(DeviceFeature feature)
        {
            if (feature == null)
                throw new ArgumentNullException("feature");

            return _methodsToCheckIfEnabled.Read(feature,
                () => _methodsToCheckIfAvailable.Read(feature, () => false));
        }

        private readonly DeviceFeatureMethodDictionary _methodsToCheckIfAvailable;
        private readonly DeviceFeatureMethodDictionary _methodsToCheckIfEnabled;
    }
}
