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

namespace Bsa.Dsp.Filters.Others
{
    // Based on original implementation described in http://pubs.acs.org/doi/abs/10.1021/ac60214a047,
    // it may be worth to review proposed Gorry's implementaion described at http://pubs.acs.org/doi/abs/10.1021/ac00205a007.
    sealed class SavitzkyGolayFilterDesigner : OnlineFilterDesigner
    {
        protected internal override IOnlineFilter CreateOther(FilterDesignSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
