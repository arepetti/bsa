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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bsa.Instrumentation
{
    /// <summary>
    /// Utilitiy methods to install Windows Performance Counters telemetry data.
    /// </summary>
    public static class WpcTelemetrySessionInstaller
    {
        /// <summary>
        /// Register all the telemetry counters defined in the specified type and in all its base types.
        /// </summary>
        /// <param name="typeContainingTelemetryDataDefinitions">
        /// The type of a class derived from <see cref="WpcTelemetrySession"/> which contains the definition of all the counters you want to use. All counters
        /// must be unregistered (with <see cref="Uninstall"/>) when application is uninstalled or updated or when you want to add new counters (progressive
        /// <em>update</em> is only supported if you create a new category, it's not possible to add new counters to an existing category).
        /// </param>
        public static void Install(Type typeContainingTelemetryDataDefinitions)
        {
            if (typeContainingTelemetryDataDefinitions == null)
                throw new ArgumentNullException("typeContainingTelemetryDataDefinitions");

            RegisterPerformanceCounters(TelemetrySession.FindTelemetryData(typeContainingTelemetryDataDefinitions));
        }

        /// <summary>
        /// Deregister all the telemetry counters defined in the specified type and in all its base types.
        /// </summary>
        /// <param name="typeContainingTelemetryDataDefinitions">
        /// The type of a class derived from <see cref="WpcTelemetrySession"/> which contains the definition of all the counters you registered
        /// with a previous call to <see cref="Install"/>.
        /// </param>
        public static void Uninstall(Type typeContainingTelemetryDataDefinitions)
        {
            if (typeContainingTelemetryDataDefinitions == null)
                throw new ArgumentNullException("typeContainingTelemetryDataDefinitions");

            DeregisterPerformanceCounters(TelemetrySession.FindTelemetryData(typeContainingTelemetryDataDefinitions));
        }

        internal static bool RegisterPerformanceCounters(IEnumerable<TelemetryData> data)
        {
            int registeredCategories = 0;

            foreach (var categoryGroup in data.GroupBy(x => x.Category))
            {
                if (!PerformanceCounterCategory.Exists(categoryGroup.Key))
                {
                    var counterData = new CounterCreationDataCollection();
                    foreach (var item in data)
                    {
                        counterData.Add(new CounterCreationData(item.Name, "", item.DataTypeToCounterType()));

                        // Base PC name convention: keep in sync with code in PerformanceCounterHolderCollection
                        if (item.CounterType == TelemetryDataType.AverageCount)
                            counterData.Add(new CounterCreationData(item.Name + "Base", "", PerformanceCounterType.AverageBase));
                    }

                    PerformanceCounterCategory.Create(categoryGroup.Key, "", PerformanceCounterCategoryType.SingleInstance, counterData);

                    ++registeredCategories;
                }
            }

            return registeredCategories > 0;
        }

        private static void DeregisterPerformanceCounters(IEnumerable<TelemetryData> data)
        {
            foreach (var categoryGroup in data.GroupBy(x => x.Category))
            {
                if (PerformanceCounterCategory.Exists(categoryGroup.Key))
                    PerformanceCounterCategory.Delete(categoryGroup.Key);
            }
        }
    }
}
