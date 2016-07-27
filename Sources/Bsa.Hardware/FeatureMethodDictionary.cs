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
using System.Reflection;

namespace Bsa.Hardware
{
    // Maps a device feature to a method in Device class to check for its availablility.
    sealed class FeatureMethodDictionary
    {
        public FeatureMethodDictionary(Device methodsContainer, string methodNameFormatString, bool mayUseMethodWithOneParameter)
        {
            _device = methodsContainer;
            _methodNameFormatString = methodNameFormatString;
            _mayUseMethodWithOneParameter = mayUseMethodWithOneParameter;
        }

        public bool Read(Feature feature, object param, Func<bool> defaultValue)
        {
            if (!feature.AssociatedDevice.IsAssignableFrom(_device.GetType()))
                throw new ArgumentException("You can determine if a feature is available only if it belongs to its associated device or one of its derived classes.", "feature");

            var method = FindMethodForFeature(feature);
            if (method == null)
                return defaultValue();

            if (method.GetParameters().Length == 0)
                return (bool)method.Invoke(_device, null);

            return (bool)method.Invoke(_device, new object[] { param });
        }

        public bool Read(Feature feature, Func<bool> defaultValue)
        {
            // If called method accepts one parameter then we pass null (unspecified), if
            // it doesn't accept any parameter then param value is simply ignored.
            return Read(feature, null, defaultValue);
        }

        private readonly Device _device;
        private readonly string _methodNameFormatString;
        private readonly bool _mayUseMethodWithOneParameter;
        private readonly Dictionary<Feature, MethodInfo> _mapping = new Dictionary<Feature, MethodInfo>();

        private MethodInfo FindMethodForFeature(Feature feature)
        {
            lock (_mapping)
            {
                MethodInfo method;
                if (_mapping.TryGetValue(feature, out method))
                    return method;

                // See FeatureCollection documentation: any public/private static/instance method is eligible, search is case-insensitive
                method = _device.GetType().GetMethod(String.Format(_methodNameFormatString, feature.EquivalentName),
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

                if (method != null && !IsMethodEligibleForInvocation(method))
                    method = null;
                
                _mapping.Add(feature, method);

                return method;
            }
        }

        private bool IsMethodEligibleForInvocation(MethodInfo method)
        {
            // See FeatureCollection documentation: method must have bool return type, it can't be generic and it MAY have one or zero parameters
            if (method.ReturnType != typeof(bool))
                return false;

            if (method.IsGenericMethod)
                return false;

            return method.GetParameters().Length <= (_mayUseMethodWithOneParameter ? 1 : 0);
        }
    }
}
