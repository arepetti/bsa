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
using Bsa.Dsp.Filters;
using Bsa.Dsp.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Dsp.Tests.Filters
{
    [TestClass]
    public sealed class FisherTests : OnlineFilterTests
    {
        [TestMethod]
        public void Chebyshev()
        {
            TestFilter(OnlineFilterDesign.Iir.ChebyshevI, false);
        }

        [TestMethod]
        public void Butterworth()
        {
            TestFilter(OnlineFilterDesign.Iir.Butterworth, false);
        }

        [TestMethod]
        public void Bessel()
        {
            TestFilter(OnlineFilterDesign.Iir.Bessel, false);
        }
    }
}
