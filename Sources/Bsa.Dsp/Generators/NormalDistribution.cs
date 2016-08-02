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
    /// <devdoc>
    /// This class is not intended to be used for statistical analisys but used internally to
    /// generate a "good enough" gaussian distribution for gaussian white noise generator.
    /// For this reason we use an easy-to-use Box-Muller transform method (for maximum speed we may even
    /// use Ziggurat algorithm.)
    /// </devdoc>
    sealed class NormalDistribution
    {
        public NormalDistribution(IGenerator<double> random)
        {
            Debug.Assert(random != null);

            _random = random;
        }

        public double Next()
        {
            Debug.Assert(_random != null);

            double u1 = _random.Next();
            double u2 = _random.Next();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        }

        private readonly IGenerator<double> _random;
    }
}