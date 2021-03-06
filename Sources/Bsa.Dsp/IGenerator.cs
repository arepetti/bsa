﻿//
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

namespace Bsa.Dsp
{
    /// <summary>
    /// Interface implemented by data generators.
    /// </summary>
    public interface IGenerator<T>
    {
        /// <summary>
        /// Generates next value.
        /// </summary>
        /// <returns>
        /// Generated value, range of this value is completely implementation defined.
        /// It may be [0..1] or within any arbitrary range.
        /// </returns>
        T Next();

        /// <summary>
        /// Resets this generator to its initial state.
        /// </summary>
        /// <remarks>
        /// This function may not be applicable for all generators, if it is not supported then
        /// it simply does nothing.
        /// </remarks>
        void Reset();
    }
}
