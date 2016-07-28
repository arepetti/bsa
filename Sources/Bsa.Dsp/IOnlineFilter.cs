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

namespace Bsa.Dsp
{
    /// <summary>
    /// Interface implemented by online fitlers whose process a samples stream one by one.
    /// </summary>
    public interface IOnlineFilter : IDisposable
    {
        /// <summary>
        /// Processes specified sample.
        /// </summary>
        /// <param name="sample">Sample to process.</param>
        /// <returns>
        /// Specified sample, fitlered according to filter's coefficients (when applicable) and
        /// current state (when applicable).
        /// </returns>
        double Process(double sample);

        /// <summary>
        /// Reset filter's state (when applicable).
        /// </summary>
        void Reset();
    }
}
