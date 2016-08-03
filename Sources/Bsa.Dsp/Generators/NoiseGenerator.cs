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
    /// Base class for noise generators.
    /// </summary>
    /// <remarks>
    /// Generated values are within the range specified in <see cref="NoiseGenerator.Range"/> property.
    /// </remarks>
    public abstract class NoiseGenerator : SamplesGenerator
    {
        /// <summary>
        /// Initializes a new instance of <see cref="NoiseGenerator"/> using the specifed
        /// random number generator.
        /// </summary>
        /// <param name="random">
        /// An instance of random number generator used to generate pseudo-random numbers
        /// to calculate the output of this noise generator. Generated numbers must be in the
        /// range [0...1].
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="random"/> is <see langword="null"/>.
        /// </exception>
        protected NoiseGenerator(IGenerator<double> random)
        {
            if (random == null)
                throw new ArgumentNullException("random");

            _random = random;
            Range = new Range<double>(0, 1);
        }

        /// <summary>
        /// Gets/sets the range of generated noise.
        /// </summary>
        /// <value>
        /// The range of the generated samples. Default value is [0...1].
        /// </value>
        public Range<double> Range
        {
            get;
            set;
        }

        /// <summary>
        /// Resets this generator to its initial state
        /// resetting the random numbers generator.
        /// </summary>
        /// <remarks>
        /// If a specific implementation does not use this random number generator or state
        /// is more complex then this function has no effect or it performs a partial reset.
        /// </remarks>
        public override void Reset()
        {
            Debug.Assert(_random != null);

            base.Reset();
            _random.Reset();
        }

        /// <summary>
        /// Generates a new random number in the range [0...1].
        /// </summary>
        /// <returns>Pseudo-random number within the range [0...1].</returns>
        protected double NextRandom()
        {
            Debug.Assert(_random != null);

            return _random.Next();
        }

        private readonly IGenerator<double> _random;
    }
}
