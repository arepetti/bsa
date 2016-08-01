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

namespace Bsa.Dsp.Generators
{
    /// <summary>
    /// Generator of a sequence of random numbers.
    /// </summary>
    /// <remarks>
    /// Unless an external random numbers generator is specified this implementation
    /// by default use <see cref="System.Random"/> pseudo-random generators. It is not a cryptographically
    /// strong random numbers generator and it has not a good distribution, you should use it only in applications
    /// where those features are not required.
    /// </remarks>
    sealed class RandomNumbersGenerator : SamplesGenerator
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RandomNumbersGenerator"/>
        /// that will use the specified <c>System.Random</c> instance to generate new random numbers.
        /// </summary>
        /// <param name="random">An instance of <c>System.Random</c> used to generate random numbers.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="random"/> is <see langword="null"/>.
        /// </exception>
        public RandomNumbersGenerator(Random random)
        {
            if (random == null)
                throw new ArgumentNullException("random");

            _randomizer = random;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RandomNumbersGenerator"/> delegating
        /// generation to the specified user-defined function.
        /// </summary>
        /// <param name="generator">
        /// User defined function to generate random numbers. Generated values must be in the range
        /// [0...1] otherwise class behavior is undefined.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="generator"/> is <see langword="null"/>.
        /// </exception>
        public RandomNumbersGenerator(Func<double> generator)
        {
            if (generator == null)
                throw new ArgumentNullException("generator");

            _generator = generator;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RandomNumbersGenerator"/>
        /// using a new <see cref="Random"/> instance to generate random numbers.
        /// </summary>
        /// <remarks>
        /// <see cref="Random"/> is not a cryptographically strong random numbers generator and it has not a good distribution,
        /// you should use it only in applications where those features are not required.
        /// </remarks>
        public RandomNumbersGenerator()
            : this(new Random())
        {
        }

        /// <summary>
        /// Generates the next random number in the range [0...1].
        /// </summary>
        /// <returns>
        /// A random number, in the range [0...1].
        /// </returns>
        public override double Next()
        {
            Debug.Assert(_generator != null || _randomizer != null);

            if (_generator == null)
                return _randomizer.Next();

            return _generator();
        }

        /// <summary>
        /// Generates a new random number within the specified range.
        /// </summary>
        /// <param name="minimum">Minimum inclusive value for the generated random number.</param>
        /// <param name="maximum">Maximum inclusive value for the generated random number.</param>
        /// <returns>
        /// A random number in the range [<paramref name="minimum"/>...<paramref name="maximum"/>].
        /// If one of the inputs is <see cref="Double.NaN"/> then output will be <see cref="Double.NaN"/>.
        /// If one of inputs is infinity then output will be the same.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="minimum"/> is greater than <paramref name="maximum"/>.
        /// </exception>
        public double Next(double minimum, double maximum)
        {
            if (minimum > maximum)
                throw new ArgumentOutOfRangeException("Minimum cannot be higher than maximum.");

            return minimum + Next() * (maximum - minimum);
        }

        private readonly Random _randomizer;
        private readonly Func<double> _generator;
    }
}
