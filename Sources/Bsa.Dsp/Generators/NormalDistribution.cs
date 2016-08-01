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
using System.Diagnostics;

namespace Bsa.Dsp.Generators
{
    sealed class NormalDistribution
    {
        public NormalDistribution(IGenerator<double> random, double mean, double standardDeviation)
        {
            Debug.Assert(random != null);
            Debug.Assert(!Double.IsNaN(mean) && !Double.IsInfinity(mean));
            Debug.Assert(standardDeviation >= 0 && !Double.IsNaN(standardDeviation) && !Double.IsInfinity(standardDeviation));

            _random = random;
            _mean = mean;
            _standardDeviation = standardDeviation;
        }

        public double Next()
        {
            Debug.Assert(_random != null);

            while (true)
            {
                var v1 = 2 * _random.Next() - 1;
                var v2 = 2 * _random.Next() - 1;

                var r = v1 * v1 + v2 * v2;
                if (r == 0.0 || r >= 1.0)
                    continue;

                double x = v1 * (Math.Sqrt(-2 * Math.Log(r) / r));
                return _mean + (_standardDeviation * x);
            }
        }

        private readonly IGenerator<double> _random;
        private readonly double _mean;
        private readonly double _standardDeviation;
    }
}