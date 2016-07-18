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
using System.Diagnostics;

namespace Bsa.Instrumentation
{
    sealed class PerformanceCounterHolderCollection : Disposable
    {
        public PerformanceCounterHolderCollection(TelemetryData[] counters)
        {
            _telemetryData = new Dictionary<TelemetryData, PerformanceCounterHolder>(counters.Length);
            foreach (var counter in counters)
                _telemetryData.Add(counter, new PerformanceCounterHolder());
        }

        public IEnumerable<TelemetryData> Data
        {
            get { return _telemetryData.Keys; }
        }

        public bool TryGetValue(TelemetryData data, out PerformanceCounterHolder value)
        {
            if (!_telemetryData.TryGetValue(data, out value))
                return false;

            // PerformanceCounterHolder exists but it's disabled, there is no need to
            // (try to) create an instance of its associated PC(s).
            if (value.IsDisabled)
                return true;

            // If caller requested this PerformanceCounterHolder then it will use it,
            // it's the right moment to (lazy) create required instances. Note that
            // in this way performance penality for this operation is spanned across program
            // execution, it's a drawback but it speed-up initialization time (especially if there are
            // many mostly unused PCs).
            LazyCreatePerformanceCounter(value, data);

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed)
                {
                    foreach (var item in _telemetryData)
                    {
                        if (item.Value.Counter != null)
                            item.Value.Counter.Dispose();

                        if (item.Value.Base != null)
                            item.Value.Base.Dispose();
                    }

                    _telemetryData.Clear();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private readonly Dictionary<TelemetryData, PerformanceCounterHolder> _telemetryData;

        private static void LazyCreatePerformanceCounter(PerformanceCounterHolder holder, TelemetryData data)
        {
            if (holder.Counter != null)
                return;

            // If counter does not exist then it's probably an installation problem:
            // * counters have not been created and TelemetrySession.Start did it but OS didn't refresh them yet;
            // * category already existed but a new version added more counters and old category has not been properly uninstalled.
            if (!PerformanceCounterCategory.CounterExists(data.Name, data.Category))
            {
                holder.IsDisabled = true;
                return;
            }

            holder.Counter = new PerformanceCounter(data.Category, data.Name, false);

            if (data.CounterType == TelemetryDataType.AverageCount)
            {
                // Base PC name convention: keep in sync with code in WpcTelemetrySessionInstaller
                holder.Base = new PerformanceCounter(data.Category, data.Name + "Base", false);

                holder.Counter.RawValue = 0;
                holder.Base.RawValue = 0;
            }
        }
    }
}
