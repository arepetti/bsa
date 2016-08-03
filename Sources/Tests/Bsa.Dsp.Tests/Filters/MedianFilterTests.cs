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
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bsa.Dsp.Filters;

namespace Bsa.Dsp.Tests.Filters
{
    [TestClass]
    public sealed class MedianFilterTests
    {
        [TestMethod]
        public void OddWindowSize()
        {
            var filter = OnlineFilterFactory.Create(OnlineFilterDesign.Median,
                new MedianFilterDesignSettings { WindowSize = 5 });

            var result = Accumulate(filter, 1, 2, 3, 4, 5, 6, 7);

            Assert.AreEqual(1, result[0]); // 1 - - - -
            Assert.AreEqual(1, result[1]); // 1 2 - - -
            Assert.AreEqual(2, result[2]); // 1 2 3 - -
            Assert.AreEqual(2, result[3]); // 1 2 3 4 -
            Assert.AreEqual(3, result[4]); // 1 2 3 4 5
            Assert.AreEqual(4, result[5]); // 2 3 4 5 6
            Assert.AreEqual(5, result[6]); // 3 4 5 6 7
        }

        [TestMethod]
        public void EvenWindowSize()
        {
            var filter = OnlineFilterFactory.Create(OnlineFilterDesign.Median,
                new MedianFilterDesignSettings { WindowSize = 4 });

            var result = Accumulate(filter, 1, 2, 3, 4, 5, 6, 7);

            Assert.AreEqual(1, result[0]); // 1 - - -
            Assert.AreEqual(1, result[1]); // 1 2 - -
            Assert.AreEqual(2, result[2]); // 1 2 3 -
            Assert.AreEqual(2, result[3]); // 1 2 3 4
            Assert.AreEqual(3, result[4]); // 2 3 4 5
            Assert.AreEqual(4, result[5]); // 3 4 5 6
            Assert.AreEqual(5, result[6]); // 4 5 6 7
        }

        private static double[] Accumulate(IOnlineFilter filter, params double[] samples)
        {
            return samples.Select(x => filter.Process(x)).ToArray();
        }
    }
}
