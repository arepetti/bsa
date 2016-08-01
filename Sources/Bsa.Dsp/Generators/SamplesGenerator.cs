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
using System.Collections.Generic;

namespace Bsa.Dsp.Generators
{
    /// <summary>
    /// Base class for a <see cref="IGenerator{T}"/> which produces <see cref="System.Double"/>.
    /// </summary>
    public abstract class SamplesGenerator : IGenerator<double>
    {
        /// <overload>
        /// Generates one or more samples.
        /// </overload>
        /// <summary>
        /// Generates next value.
        /// </summary>
        /// <returns>
        /// Generated value, range of this value is completely implementation defined. It may be [0..1] or within any arbitrary range.
        /// </returns>
        public abstract double Next();

        /// <summary>
        /// Generates next <paramref name="count"/> samples.
        /// </summary>
        /// <param name="count">Number of samples to generate, if 0 then an empty array is returned.</param>
        /// <returns>
        /// An array with <paramref name="count"/> sequentially generated samples.
        /// </returns>
        /// <remarks>
        /// See <see cref="Next()"/> for details about range of generated samples.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="count"/> is less than zero.
        /// </exception>
        public virtual double[] Next(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            var result = new double[count];
            for (int i = 0; i < count; ++i)
                result[i] = Next();

            return result;
        }

        /// <summary>
        /// Generates the specified number of samples storing them in the given array,
        /// starting from a specified index.
        /// </summary>
        /// <param name="data">Existing array where generated samples will be stored.</param>
        /// <param name="startIndex">Index, within the array <paramref name="data"/>, where first sample will be stored.</param>
        /// <param name="count">
        /// Number of samples to generate, they will be stored starting from <paramref name="startIndex"/> one after the other
        /// at increasing locations.
        /// </param>
        /// <remarks>
        /// See <see cref="Next()"/> for details about range of generated samples.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="data"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="startIndex"/> is less than zero or greater than the last array item index.
        /// <br/>-or<br/>
        /// If <paramref name="count"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="data"/>  does not contain at least <paramref name="count"/> items
        /// after <paramref name="startIndex"/>.
        /// </exception>
        public virtual void Next(double[] data, int startIndex, int count)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (startIndex < 0 || startIndex >= data.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if (startIndex + count > data.Length)
                throw new ArgumentException("Array 'data' is not big enough to store 'count' samples after 'startIndex'");

            for (int i = 0; i < count; ++i)
                data[startIndex + i] = Next();
        }

        /// <summary>
        /// Generates an infinite sequence of samples.
        /// </summary>
        /// <returns>
        /// An enumerator to go through an infinite sequence of samples. Note that you will need a break
        /// condition to stop this enumeration, sequence is infinite.
        /// </returns>
        /// <remarks>
        /// Derived classes should override this method if an infinite sequence is not applicable and there is
        /// some sort of exit-condition to apply. See <see cref="Next()"/> for details about range of generated samples.
        /// </remarks>
        public virtual IEnumerable<double> Infinite()
        {
            while (true)
                yield return Next();
        }
    }
}
