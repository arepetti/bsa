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
using System.Globalization;
using Bsa.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Tests
{
    [TestClass]
    public sealed class IntervalTests
    {
        [TestMethod]
        public void Create()
        {
            var start = new DateTime(2016, 07, 06);
            var duration = TimeSpan.FromHours(1);

            var interval1 = new Interval(start, duration);

            Assert.AreEqual(start, interval1.Start);
            Assert.AreEqual(duration, interval1.Length);
            Assert.AreEqual(start + duration, interval1.End);

            var interval2 = new Interval(start, start + duration);
            Assert.AreEqual(interval1.End, interval2.End);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void CannotCreateNullInterval()
        {
            new Interval(null, null);
        }

        [TestMethod]
        public void OpenIntervals()
        {
            Assert.AreEqual(null, new Interval(DateTime.Now, (DateTime?)null).Length);
        }

        [TestMethod]
        public void ConvertFromToString()
        {
            var converter = new IntervalTypeConverter();
            var interval = new Interval(new DateTime(2016, 07, 06), TimeSpan.FromHours(1));

            var asString = converter.ConvertToString(null, new CultureInfo("it-IT"), interval);
            var fromString = (Interval)converter.ConvertFromString(null, CultureInfo.InvariantCulture, asString);

            Assert.AreEqual(fromString, interval);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(interval.ToString())); // Raw conversion, debugging purposes
        }

        [TestMethod]
        public void Equals()
        {
            var start = new DateTime(2016, 07, 06);
            var duration = TimeSpan.FromHours(1);

            var interval = new Interval(start, duration);

            Assert.AreEqual(new Interval(start, start + duration), interval);
            Assert.AreNotEqual(new Interval(start.AddDays(1), duration), interval);
        }

        [TestMethod]
        public void ContainsInstant()
        {
            var start = new DateTime(2016, 07, 06);
            var duration = TimeSpan.FromHours(1);
            var interval = new Interval(start, duration);

            Assert.IsFalse(interval.Contains(start.AddHours(-0.5)));
            Assert.IsTrue(interval.Contains(start.AddHours(0.5)));
            Assert.IsFalse(interval.Contains(start.AddHours(1.5)));
        }

        [TestMethod]
        public void ContainsInterval()
        {
            var start = new DateTime(2016, 07, 06);
            var duration = TimeSpan.FromHours(1);

            var interval = new Interval(start, duration);

            Assert.IsTrue(interval.Contains(new Interval(start.AddSeconds(1), TimeSpan.FromSeconds(1))));
            Assert.IsFalse(interval.Contains(new Interval(start.AddSeconds(-1), TimeSpan.FromSeconds(2))));
            Assert.IsFalse(interval.Contains(new Interval(start.AddSeconds(-2), TimeSpan.FromSeconds(1))));
            Assert.IsFalse(interval.Contains(new Interval(start + duration + TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))));
        }

        [TestMethod]
        public void Intersects()
        {
            var start = new DateTime(2016, 07, 06);
            var duration = TimeSpan.FromHours(1);

            var interval = new Interval(start, duration);

            var before = new Interval(start.AddDays(-1), duration);
            var intersectsBeginning = new Interval(start.AddHours(-0.5), duration);
            var inner = new Interval(start.AddSeconds(10), TimeSpan.FromSeconds(10));
            var intersectsAfter = new Interval(start + duration - TimeSpan.FromHours(0.5), duration);
            var after = new Interval(start.AddDays(+1), duration);

            Assert.IsFalse(interval.Intersects(before));
            Assert.IsTrue(interval.Intersects(intersectsBeginning));
            Assert.IsTrue(interval.Intersects(inner));
            Assert.IsTrue(interval.Intersects(intersectsAfter));
            Assert.IsFalse(interval.Intersects(after));
        }

        [TestMethod]
        public void Intersect()
        {
            var start = new DateTime(2016, 07, 06);
            var duration = TimeSpan.FromHours(1);

            var interval = new Interval(start, duration);

            var before = new Interval(start.AddDays(-1), duration);
            var beginning = new Interval(start.AddHours(-0.5), duration);
            var inner = new Interval(start.AddSeconds(10), TimeSpan.FromSeconds(10));
            var end = new Interval(start + duration - TimeSpan.FromHours(0.5), duration);
            var after = new Interval(start.AddDays(+1), duration);

            var intersectionBefore = Interval.Intersect(interval, before);
            var intersectionBeginning = Interval.Intersect(interval, beginning);
            var intersectionInner = Interval.Intersect(interval, inner);
            var intersectionEnd = Interval.Intersect(interval, end);
            var intersectionAfter = Interval.Intersect(interval, after);

            Assert.IsTrue(intersectionBefore == Interval.Null);
            Assert.IsTrue(intersectionBeginning.Start == interval.Start && intersectionBeginning.Length == TimeSpan.FromHours(0.5));
            Assert.IsTrue(intersectionInner == inner);
            Assert.IsTrue(intersectionEnd.Start == end.Start && intersectionEnd.Length == TimeSpan.FromHours(0.5));
            Assert.IsTrue(intersectionAfter == Interval.Null);
        }

        [TestMethod]
        public void Comparisons()
        {
            var start = new DateTime(2016, 07, 06);
            var duration = TimeSpan.FromHours(1);
            var before = start.AddDays(-1);
            var after = start.AddDays(+1);

            var interval = new Interval(start, duration);

            Assert.IsTrue(before < interval);
            Assert.IsTrue(start <= interval);
            Assert.IsFalse(after < interval);

            Assert.IsTrue(after > interval);
            Assert.IsTrue(start + duration >= interval);
            Assert.IsFalse(before > interval);

            Assert.IsTrue(interval == new Interval(start, duration));
            Assert.IsTrue(interval != new Interval(start, TimeSpan.Zero));

            Assert.IsTrue(interval < after);
            Assert.IsTrue(interval <= start + duration);
            Assert.IsFalse(interval > after);
            
            Assert.IsTrue(interval > before);
            Assert.IsTrue(interval >= start);
            Assert.IsFalse(interval < before);
        }
    }
}
