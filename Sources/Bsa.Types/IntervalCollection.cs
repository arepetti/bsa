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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Bsa
{
    /// <summary>
    /// Represents a collection of <see cref="Interval"/>.
    /// </summary>
    [Serializable]
    public class IntervalCollection : ObservableCollection<Interval>
    {
        /// <summary>
        /// Determines if any interval of this collection contains the specified instant.
        /// </summary>
        /// <param name="time">Instant to search within this collection.</param>
        /// <returns>
        /// <see langword="true"/> if any interval of this collection contains specified instant.
        /// </returns>
        /// <seealso cref="Interval.Contains(DateTime)"/>
        public bool Contains(DateTime time)
        {
            return Items.Any(x => x.Contains(time));
        }

        /// <summary>
        /// Intersects all intervals of this collection with each other and return those intersections.
        /// </summary>
        /// <returns>
        /// All intersections between each interval of this collection, each intersection is returned exactly once. Open intervals
        /// (without beginning or end) are ignored.
        /// </returns>
        /// <seealso cref="Interval.Intersect"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification="It performs a possibly long operation")]
        public IEnumerable<Interval> GetIntersections()
        {
            var intervals = Items.Where(x => x.Length.HasValue).ToArray();
            for (int i = 0; i < intervals.Length; ++i)
            {
                for (int j = i + 1; j < intervals.Length; ++j)
                {
                    Interval intersection = Interval.Intersect(intervals[i], intervals[j]);
                    if (intersection != Interval.Null)
                        yield return intersection;
                }
            }
        }
    }
}
