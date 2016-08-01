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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bsa.Dsp.Generators
{
    // TODO: documentation and tests...
    public sealed class WhiteGaussianNoiseGenerator : NoiseGenerator
    {
        public WhiteGaussianNoiseGenerator(IGenerator<double> random, double mean, double standardDeviation)
            : base(random)
        {
            if (Double.IsNaN(mean) || Double.IsInfinity(mean))
                throw new ArgumentOutOfRangeException("mean");

            if (standardDeviation < 0 || Double.IsNaN(standardDeviation) || Double.IsInfinity(standardDeviation))
                throw new ArgumentOutOfRangeException("standardDeviation");

            _normalDistribution = new NormalDistribution(random, mean, standardDeviation);
        }

        public override double Next()
        {
            return _normalDistribution.Next();
        }

        private readonly NormalDistribution _normalDistribution;
    }
}
