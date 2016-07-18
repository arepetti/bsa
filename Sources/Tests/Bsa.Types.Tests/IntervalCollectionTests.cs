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
// You should have received a copy of the GNU Lesse General Public License
// along with BSA-F.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Tests
{
    [TestClass]
    public class IntervalCollectionTests
    {
        [TestMethod]
        public void Contains()
        {
            var now = DateTime.UtcNow;

            var intervals = new IntervalCollection();
            intervals.Add(new Interval(now, TimeSpan.FromHours(1)));
            intervals.Add(new Interval(now.AddDays(10), TimeSpan.FromHours(1)));

            Assert.IsTrue(intervals.Contains(now));
            Assert.IsTrue(intervals.Contains(now.AddMinutes(10)));
            Assert.IsFalse(intervals.Contains(now.AddMinutes(-10)));
            Assert.IsFalse(intervals.Contains(now.AddDays(5)));
            Assert.IsFalse(intervals.Contains(now.AddDays(11)));
        }

        [TestMethod]
        public void Intersections()
        {
            var now = DateTime.UtcNow;

            var intervals = new IntervalCollection();
            intervals.Add(new Interval(now, TimeSpan.FromHours(1)));
            intervals.Add(new Interval(now.AddDays(10), TimeSpan.FromHours(1)));

            Assert.IsTrue(intervals.GetIntersections().Count() == 0);

            intervals.Clear();
            intervals.Add(new Interval(now, TimeSpan.FromHours(2)));
            intervals.Add(new Interval(now.AddHours(-0.5), TimeSpan.FromHours(1)));

            var intersection = intervals.GetIntersections().Single();
            Assert.AreEqual(now, intersection.Start);
            Assert.AreEqual(TimeSpan.FromHours(0.5), intersection.Length);
        }
    }
}
