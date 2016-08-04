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

namespace Bsa.Dsp.Processors
{
    /// <summary>
    /// Represents an on-line processor which applies a simple <em>a + bx</em> transformation.
    /// </summary>
    public sealed class LinearTransformation : Processor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LinearTransformation"/>.
        /// </summary>
        public LinearTransformation()
        {
            Slope = 1;
        }

        /// <summary>
        /// Gets/sets the offset of this transformation.
        /// </summary>
        /// <value>
        /// The offset of this transformation,  <em>a</em> parameter in <em>a + bx</em>.
        /// Default value is 0.
        /// </value>
        public double Offset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the slope of this transformation.
        /// </summary>
        /// <value>
        /// The slope of this transformation,  <em>b</em> parameter in <em>a + bx</em>.
        /// Default value is 1.
        /// </value>
        public double Slope
        {
            get;
            set;
        }

        /// <summary>
        /// Processes specified sample.
        /// </summary>
        /// <param name="sample">Sample to process.</param>
        /// <returns>
        /// Specified sample processed by this <c>Processor</c>.
        /// </returns>
        protected override double ProcessCore(double sample)
        {
            return Offset + Slope * sample;
        }
    }
}
