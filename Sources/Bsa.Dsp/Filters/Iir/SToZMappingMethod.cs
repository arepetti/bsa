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

namespace Bsa.Dsp.Filters.Iir
{
    /// <devdoc>
    /// Used by FisherMethodFilterDesigner to determine which method must be used to
    /// map poles and zeros from S to Z plane.
    /// </devdoc>
    enum SToZMappingMethod
    {
        /// <devdoc>
        /// http://en.wikipedia.org/wiki/Bilinear_transform.
        /// </devdoc>
        BilinearTransform,

        /// <devdoc>
        /// http://en.wikipedia.org/wiki/Matched_Z-transform_method.
        /// </devdoc>
        MatchedZTransform
    }
}
