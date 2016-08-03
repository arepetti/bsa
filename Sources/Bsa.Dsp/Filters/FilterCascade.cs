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
// along with BSA-F. If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Diagnostics;

namespace Bsa.Dsp.Filters
{
    sealed class FilterCascade : IOnlineFilter
    {
        public FilterCascade(params IOnlineFilter[] filters)
        {
            Debug.Assert(filters != null);

            _filters = filters;
            IsEnabled = true;
        }

        public bool IsEnabled
        {
            get;
            set;
        }

        public double Process(double sample)
        {
            Debug.Assert(_filters != null);

            if (!IsEnabled)
                return sample;

            foreach (var filter in _filters)
                sample = filter.Process(sample);

            return sample;
        }

        public void Reset()
        {
            Debug.Assert(_filters != null);

            foreach (var filter in _filters)
                filter.Reset();
        }

        void IDisposable.Dispose()
        {
            Debug.Assert(_filters != null);

            foreach (var filter in _filters)
                filter.Dispose();

            GC.SuppressFinalize(this);
        }

        private readonly IOnlineFilter[] _filters;
    }
}
