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
    /// <item>Method cannot have parameters (but see later for an exception).</item>
    /// <item>Method must return <see cref="Boolean"/>.</item>
    /// <item>Method must not be a generic method.</item>
    /// <item>
    /// These functions must not be overloaded, if there is more than one function with the same name then behavior is unspecified
    /// (the right function may be called or just ignored and never called.)
    /// </item>
    /// <item>
    /// Method name must have prefix <c>IsFeature</c> followed by feature's equivalent name and then a suffix to indicate
    /// if method is used to determine whether a feature is available (<c>IsFeature*Available</c>) or enabled (<c>IsFeature*Enabled</c>).
    /// Name matching is case insensitive. Some features require an additional method to peform a specific operation, in those cases
    /// method signature is the same (with a <c>bool</c> return value which may be ignored) and name is <c>Perform*</c>.
    /// </item>
    /// <item>
    /// <c>Perform*</c> may accept an optional parameter of type <c>Object</c>. It's optional (you can both declare the method with
    /// this parameter even if it will never be used or do not declare it when used). If you declare the method with the optional
    /// parameter and it's not specified then it will be <see langword="null"/>.
    /// </item>
    /// <item>
    /// Derived classes may override base class method if it is virtual but hiding base method signature is not supported.
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
            _methodsToCheckIfAvailable = new DeviceFeatureMethodDictionary(device, "IsFeature{0}Available", mayUseMethodWithOneParameter: false);
            _methodsToCheckIfEnabled = new DeviceFeatureMethodDictionary(device, "IsFeature{0}Enabled", mayUseMethodWithOneParameter: false);
            _methodsToInvoke = new DeviceFeatureMethodDictionary(device, "Perform{0}", mayUseMethodWithOneParameter: true);
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
        public bool IsAvailable(Feature feature)
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
        /// application behavior is undefined (or, better, use <see cref="IsAvailableAndEnabled"/> instead of this function which is designed to be
        /// used when availability is checked once and if enabled is checked multiple times.)
        /// </remarks>
        public bool IsEnabled(Feature feature)
        {
            if (feature == null)
                throw new ArgumentNullException("feature");

            return _methodsToCheckIfEnabled.Read(feature,
                () => _methodsToCheckIfAvailable.Read(feature, () => false));
        }

        /// <summary>
        /// Combines the calls to <see cref="IsAvailable"/> and <see cref="IsEnabled"/>.
        /// </summary>
        /// <param name="feature">The feature you want to check.</param>
        /// <returns>
        /// <see langword="true"/> if specified feature is both available and enabled.
        /// </returns>
        public bool IsAvailableAndEnabled(Feature feature)
        {
            if (feature == null)
                throw new ArgumentNullException("feature");

            if (!_methodsToCheckIfAvailable.Read(feature, () => false))
                return false;

            return _methodsToCheckIfEnabled.Read(feature, () => false);
        }

        /// <summary>
        /// Performs an operation related to a specific feature.
        /// </summary>
        /// <param name="feature">The feature you want to <em>perform</em>.</param>
        /// <param name="param">An optional parameter for the method to invoke.</param>
        /// <returns>
        /// Return value is implementation defined, if required method is not present then this function
        /// always return <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If  <paramref name="feature"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If specified feature is not associated with device linked with this object (or to one of its derived classes).
        /// </exception>
        /// <remarks>
        /// This method do not check if feature is available and enabled before attempting to perform specified operation.
        /// </remarks>
        public bool Perform(Feature feature, object param = null)
        {
            if (feature == null)
                throw new ArgumentNullException("feature");

            return _methodsToInvoke.Read(feature, param, () => false);
        }

        private readonly DeviceFeatureMethodDictionary _methodsToCheckIfAvailable;
        private readonly DeviceFeatureMethodDictionary _methodsToCheckIfEnabled;
        private readonly DeviceFeatureMethodDictionary _methodsToInvoke;
    }
}
