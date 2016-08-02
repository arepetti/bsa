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

namespace Bsa.Dsp.Generators
{
    /// <summary>
    /// Represents a Gaussian white noise generator.
    /// </summary>
    /// <remarks>
    /// Please note that <see cref="NoiseGenerator.Range"/> is ignored and generated values are
    /// 66% of them are inside the range [-1...1].
    /// </remarks>
    public sealed class WhiteGaussianNoiseGenerator : NoiseGenerator
    {
        /// <summary>
        /// Initializes a new instance of <see cref="WhiteGaussianNoiseGenerator"/>.
        /// </summary>
        /// <param name="random">
        /// Generator from which obtain random samples used to build a white noise stream. If you do not have any
        /// special distribution requirement you may simply use <see cref="RandomNumbersGenerator"/> and its default
        /// parameterless constructor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="random"/> is <see langword="null"/>.
        /// </exception>
        public WhiteGaussianNoiseGenerator(IGenerator<double> random)
            : base(random)
        {
            _normalDistribution = new NormalDistribution(random);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="WhiteGaussianNoiseGenerator"/>.
        /// </summary>
        /// <param name="random">
        /// Generator from which obtain random samples used to build a white noise stream. If you do not have any
        /// special distribution requirement you may simply use <see cref="RandomNumbersGenerator"/> and its default
        /// parameterless constructor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="random"/> is <see langword="null"/>.
        /// </exception>
        public WhiteGaussianNoiseGenerator(Func<double> random)
            : this(new DelegatedGenerator(random))
        {
        }

        /// <summary>
        /// Generates next sample of white noise.
        /// </summary>
        /// <returns>Next generated white noise sample.</returns>
        public override double Next()
        {
            return _normalDistribution.Next();
        }

        private readonly NormalDistribution _normalDistribution;

        private sealed class DelegatedGenerator : IGenerator<double>
        {
            public DelegatedGenerator(Func<double> function)
            {
                if (function == null)
                    throw new ArgumentNullException("function");

                _function = function;
            }

            public double Next()
            {
                return _function();
            }

            private readonly Func<double> _function;
        }
    }
}
