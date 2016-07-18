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
    sealed class DeviceFeatureMethodDictionary
    {
        public DeviceFeatureMethodDictionary(Device methodsContainer, string methodNameFormatString)
        {
            _device = methodsContainer;
            _methodNameFormatString = methodNameFormatString;
        }

        public bool Read(DeviceFeature feature, Func<bool> defaultValue)
        {
            if (!feature.AssociatedDevice.IsAssignableFrom(_device.GetType()))
                throw new ArgumentException("You can determine if a feature is available only if it belongs to its associated device or one of its derived classes.", "feature");

            var method = FindMethodForFeature(feature);
            if (method == null)
                return defaultValue();

            return (bool)method.Invoke(_device, null);
        }

        private readonly Device _device;
        private readonly string _methodNameFormatString;
        private readonly Dictionary<DeviceFeature, MethodInfo> _mapping = new Dictionary<DeviceFeature, MethodInfo>();

        private MethodInfo FindMethodForFeature(DeviceFeature feature)
        {
            lock (_mapping)
            {
                MethodInfo method;
                if (_mapping.TryGetValue(feature, out method))
                    return method;

                // Any public/private static/instance method is eligible, search is performed case-insensitive, but
                // it must be parameterless and have bool return type.
                method = _device.GetType().GetMethod(String.Format(_methodNameFormatString, feature.EquivalentName),
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

                if (method != null && (method.GetParameters().Length != 0 || method.ReturnType != typeof(bool)))
                    method = null;

                _mapping.Add(feature, method);

                return method;
            }
        }
    }
}
