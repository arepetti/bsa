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
    /// Extension methods for <see cref="IOnlineFilter"/> objects.
    /// </summary>
    public static class OnlineProcessorExtensions
    {
        /// <summary>
        /// Processes all specified samples and returns a new array
        /// containing processed samples.
        /// </summary>
        /// <param name="processor">Processor instance to use with <paramref name="samples"/>.</param>
        /// <param name="samples">An array of samples to process.</param>
        /// <returns>
        /// A newly allocated array of samples containing all processed samples.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="processor"/> is <see langword="null"/>.
        /// <br/>-or-<br/>
        /// If <paramref name="samples"/> is <see langword="null"/>.
        /// </exception>
        public static double[] Process(this IOnlineProcessor processor, double[] samples)
        {
            if (processor == null)
                throw new ArgumentNullException("processor");

            if (samples == null)
                throw new ArgumentNullException("samples");

            double[] result = new double[samples.Length];
            
            for (int i = 0; i < result.Length; ++i)
                result[i] = processor.Process(samples[i]);

            return result;
        }

        /// <summary>
        /// Processes all specified samples and returns a new array
        /// containing processed samples.
        /// </summary>
        /// <param name="processor">Processor instance to use with <paramref name="samples"/>.</param>
        /// <param name="samples">An array of samples to process.</param>
        /// <param name="startIndex">Index of the first sample to process.</param>
        /// <param name="count">Number of samples to process.</param>
        /// <returns>
        /// A newly allocated array of samples containing all processed samples,
        /// array size is <paramref name="count"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="processor"/> is <see langword="null"/>.
        /// <br/>-or-<br/>
        /// If <paramref name="samples"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="startIndex"/> is outside the bounds of <paramref name="samples"/> array.
        /// <br/>-or-<br/>
        /// If <paramref name="count"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="samples"/> does not contain at least <paramref name="count"/>
        /// samples after <paramref name="startIndex"/>.
        /// </exception>
        public static double[] Process(this IOnlineProcessor processor, double[] samples, int startIndex, int count)
        {
            if (processor == null)
                throw new ArgumentNullException("processor");

            if (samples == null)
                throw new ArgumentNullException("samples");

            if (startIndex < 0 || startIndex >= samples.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if (startIndex + count > samples.Length)
                throw new ArgumentException("There are not at least 'count' samples after 'startIndex' in 'samples' array.");

            double[] result = new double[count];

            for (int i = 0; i < count; ++i)
                result[i] = processor.Process(samples[startIndex + i]);

            return result;
        }

        /// <summary>
        /// Processes all specified samples and stores the result in the same
        /// input array.
        /// </summary>
        /// <param name="processor">Processor instance to use with <paramref name="samples"/>.</param>
        /// <param name="samples">An array of samples to process.</param>
        /// <returns>
        /// A newly allocated array of samples containing all processed samples.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="processor"/> is <see langword="null"/>.
        /// <br/>-or-<br/>
        /// If <paramref name="samples"/> is <see langword="null"/>.
        /// </exception>
        public static void ProcessInPlace(this IOnlineProcessor processor, double[] samples)
        {
            if (processor == null)
                throw new ArgumentNullException("processor");

            if (samples == null)
                throw new ArgumentNullException("samples");

            for (int i = 0; i < samples.Length; ++i)
                samples[i] = processor.Process(samples[i]);
        }

        /// <summary>
        /// Processes all specified samples and stores the result in the same
        /// input array.
        /// </summary>
        /// <param name="processor">Processor instance to use with <paramref name="samples"/>.</param>
        /// <param name="samples">An array of samples to process.</param>
        /// <param name="startIndex">Index of the first sample to process.</param>
        /// <param name="count">Number of samples to process.</param>
        /// <returns>
        /// A newly allocated array of samples containing all processed samples.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="processor"/> is <see langword="null"/>.
        /// <br/>-or-<br/>
        /// If <paramref name="samples"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="startIndex"/> is outside the bounds of <paramref name="samples"/> array.
        /// <br/>-or-<br/>
        /// If <paramref name="count"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="samples"/> does not contain at least <paramref name="count"/>
        /// samples after <paramref name="startIndex"/>.
        /// </exception>
        public static void ProcessInPlace(this IOnlineProcessor processor, double[] samples, int startIndex, int count)
        {
            if (processor == null)
                throw new ArgumentNullException("processor");

            if (samples == null)
                throw new ArgumentNullException("samples");

            if (startIndex < 0 || startIndex >= samples.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if (startIndex + count > samples.Length)
                throw new ArgumentException("There are not at least 'count' samples after 'startIndex' in 'samples' array.");

            for (int i = startIndex; i < startIndex + count; ++i)
                samples[i] = processor.Process(samples[i]);
        }
    }
}
