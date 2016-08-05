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

namespace Bsa.Dsp.Processors
{
    /// <summary>
    /// On-line data processor to trim values within a specified range.
    /// </summary>
    /// <remarks>
    /// This processor does  not perform any validation on its settings,
    /// it is caller responsibility to set <see cref="LowerBoundary"/> and <see cref="UpperBoundary"/>
    /// properties to valid and coherent values.
    /// </remarks>
    public sealed class Threshold : Processor
    {
        /// <summary>
        /// Gets/sets the type of threshold should be applied.
        /// </summary>
        /// <value>
        /// The type of threshold should be applied to determine if a sample
        /// is within allowed range. Type of threshold also determines which
        /// and how <see cref="LowerBoundary"/> and <see cref="UpperBoundary"/>
        /// are used.
        /// </value>
        public ThresholdType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the lower boundary.
        /// </summary>
        /// <value>
        /// The lower boundary of the range, it is used by
        /// <see cref="ThresholdType.GreaterThan"/>, <see cref="ThresholdType.InRange"/> and
        /// by <see cref="ThresholdType.OutOfRange"/>. Default value is 0.
        /// </value>
        public double LowerBoundary
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the upper boundary.
        /// </summary>
        /// <value>
        /// The upper boundary of the range, it is used by
        /// <see cref="ThresholdType.LessThan"/>, <see cref="ThresholdType.InRange"/> and
        /// by <see cref="ThresholdType.OutOfRange"/>. Default value is 0.
        /// </value>
        public double UpperBoundary
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the value of <see cref="LowerBoundary"/> and <see cref="UpperBoundary"/>.
        /// </summary>
        /// <value>
        /// This property always returns the value of <see cref="LowerBoundary"/> but it sets
        /// both properties <see cref="LowerBoundary"/> and <see cref="UpperBoundary"/> to the same
        /// value. It is useful to have just one generic setter for <see cref="ThresholdType.LessThan"/>
        /// and <see cref="ThresholdType.GreaterThan"/>, it should not be used for other threshold types.
        /// </value>
        public double Level
        {
            get { return LowerBoundary; }
            set
            {
                LowerBoundary = value;
                UpperBoundary = value;
            }
        }

        /// <summary>
        /// Indicates whether output sample is clipped input sample
        /// or an indicator to know if it is inside required range.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if output sample is an indicator:
        /// 1 if input sample were inside required range otherwise 0.
        /// <see langword="false"/> if output sample is the input sample clipped
        /// to fall within the required range (according to <see cref="Threshold.Type"/>,
        /// <see cref="LowerBoundary"/> and <see cref="UpperBoundary"/>.
        /// Default value is <see langword="false"/>.
        /// </value>
        public bool Indicator
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
            if (Indicator)
                return IsInsideRange(sample) ? 1 : 0;

            return Trim(sample);
        }

        private double Trim(double sample)
        {
            switch (Type)
            {
                case ThresholdType.LessThan:
                    return Math.Min(UpperBoundary, sample);
                case ThresholdType.GreaterThan:
                    return Math.Max(LowerBoundary, sample);
                case ThresholdType.InRange:
                    return Mathx.Clip(LowerBoundary, UpperBoundary, sample);
                case ThresholdType.OutOfRange:
                    if (sample > LowerBoundary && sample < LowerBoundary + (UpperBoundary - LowerBoundary) / 2)
                        return LowerBoundary;

                    if (sample < UpperBoundary && sample >= LowerBoundary + (UpperBoundary - LowerBoundary) / 2)
                        return UpperBoundary;

                    return sample;
                default:
                    throw new System.ComponentModel.InvalidEnumArgumentException("Type", (int)Type, typeof(ThresholdType));
            }
        }

        private bool IsInsideRange(double sample)
        {
            switch (Type)
            {
                case ThresholdType.LessThan:
                    return sample <= UpperBoundary;
                case ThresholdType.GreaterThan:
                    return sample >= LowerBoundary;
                case ThresholdType.InRange:
                    return sample >= LowerBoundary && sample <= UpperBoundary;
                case ThresholdType.OutOfRange:
                    return sample < LowerBoundary || sample > UpperBoundary;
                default:
                    throw new System.ComponentModel.InvalidEnumArgumentException("Type", (int)Type, typeof(ThresholdType));
            }
        }
    }
}
