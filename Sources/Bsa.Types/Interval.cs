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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Bsa
{
    /// <summary>
    /// Represents an interval (a period of time with a beginning and a duration, both optional).
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(Design.IntervalTypeConverter))]
    [DebuggerDisplay("{Start} {End}")]
    public class Interval : IEquatable<Interval>
    {
        /// <summary>
        /// A null-interval which has not beginning and has not end.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This type is immutable.")]
        public static readonly Interval Null = new Interval(null, null, 0);

        /// <overloads>
        /// Creates a new <see cref="Interval"/> object.
        /// </overloads>
        /// <summary>
        /// Creates a new <see cref="Interval"/> object specifying its beginning and its end.
        /// </summary>
        /// <param name="start">
        /// Beginning of the interval or <see langword="null"/> if this interval will not have a beginning.
        /// </param>
        /// <param name="end">
        /// End of the interval or <see langword="null"/> if this interval will not have an end.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="end"/> is before <paramref name="start"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If both <paramref name="start"/> and <paramref name="end"/> are <see langword="null"/>.
        /// </exception>
        public Interval(DateTime? start, DateTime? end)
        {
            if (!start.HasValue && !end.HasValue)
                throw new ArgumentException("Cannot create an interval without start and without end.");

            if (start.HasValue && end.HasValue && end.Value < start.Value)
                throw new ArgumentOutOfRangeException("end", "End of the interval cannot be before its start.");

            _start = start;
            _end = end;
        }

        /// <summary>
        /// Creates a new <see cref="Interval"/> object specifying its beginning and its duration.
        /// </summary>
        /// <param name="start">
        /// Beginning of the interval.
        /// </param>
        /// <param name="duration">
        /// Duration of the interval, this value cannot be negative.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="duration"/> is negative (less than <see cref="TimeSpan.Zero"/>).
        /// <br/>-or-<br/>
        /// If <paramref name="duration"/> after <paramref name="start"/> results in a value greater than <see cref="DateTime.MaxValue"/>.
        /// </exception>
        public Interval(DateTime start, TimeSpan duration)
        {
            if (duration < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("duration", "End of the interval cannot be before its start.");

            _start = start;
            _end = start + duration;
        }

        /// <summary>
        /// Gets the beginning of this interval.
        /// </summary>
        /// <value>
        /// The beginning of this interval or <see langword="null"/> if this interval has not a defined beginning.
        /// </value>
        public DateTime? Start
        {
            get { return _start; }
        }

        /// <summary>
        /// Gets the end of this interval.
        /// </summary>
        /// <value>
        /// The end of this interval or <see langword="null"/> if this interval has not a defined end.
        /// </value>
        public DateTime? End
        {
            get { return _end; }
        }

        /// <summary>
        /// Gets the length (duration) of this interval.
        /// </summary>
        /// <value>
        /// The length of this interval or <see langword="null"/> is this interval has not a defined beginning and end.
        /// This value cannot be negative.
        /// </value>
        public TimeSpan? Length
        {
            get
            {
                if (!Start.HasValue || !End.HasValue)
                    return null;

                return End.Value - Start.Value;
            }
        }

        /// <summary>
        /// Determines if this interval contains the specified instant.
        /// </summary>
        /// <param name="value">Istant to verify if it belongs to this interval.</param>
        /// <returns>
        /// <see langword="true"/> if this interval contains the specified instant. Bounds are inclusive: if
        /// <paramref name="value"/> euqals to <see cref="Start"/> then this function returns <see langword="true"/>.
        /// </returns>
        public bool Contains(DateTime value)
        {
            if (Start.HasValue && value < Start.Value)
                return false;

            if (End.HasValue && value > End.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Determines if this interval completely contains the specified interval.
        /// </summary>
        /// <param name="other">The interval to verify if it is contained in this interval.</param>
        /// <returns>
        /// <see langword="true"/> this interval completely contains the specified interval (bounds are inclusive).
        /// If one bound is unspecified then it must be unspecified in both intervals (<see langword="this"/> e <paramref name="other"/>),
        /// intersections between open/bounded intervals are not supported (and this function always returns <see langword="false"/>).
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="other"/> is <see langword="null"/>.
        /// </exception>
        public bool Contains(Interval other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            if (other.Start.HasValue && this.Start.HasValue)
            {
                if (this.Start.Value > other.Start.Value)
                    return false;
            }

            if (other.End.HasValue && this.End.HasValue)
            {
                if (this.End.Value < other.End.Value)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if this interval intersects the specified interval.
        /// </summary>
        /// <param name="other">The other interval to verify if it intersects this interval.</param>
        /// <returns>
        /// <see langword="true"/> if this interval intersects the specified interval. If one interval has an open bound
        /// then it is considered to be <em>infinite</em>. This function cannot be used to compare <see cref="Null"/> intervals.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="other"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If this interval or <paramref name="other"/> are <see cref="Null"/> intervals.
        /// </exception>
        public bool Intersects(Interval other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            DateTime thisLeft = this.Left(), thisRight = this.Right();
            DateTime otherLeft = other.Left(), otherRight = other.Right();

            if (otherRight >= thisLeft && otherRight <= thisRight)
                return true;

            if (otherLeft >= thisLeft && otherLeft <= thisRight)
                return true;

            if (thisRight >= otherLeft && thisRight <= otherRight)
                return true;

            if (thisLeft >= otherLeft && thisLeft <= otherRight)
                return true;

            return false;
        }

        /// <summary>
        /// Determines if this interval intersects the specified interval.
        /// </summary>
        /// <param name="start">Beginning of the interval to verify if it intersects this interval.</param>
        /// <param name="duration">Duration of the interval to verify if it intersects this interval.</param>
        /// <returns>
        /// <see langword="true"/> if this interval intersects the specified interval. If this interval has an open bound
        /// then it is considered to be <em>infinite</em>. This function cannot be used to compare <see cref="Null"/> intervals.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If this interval is <see cref="Null"/>.
        /// </exception>
        public bool Intersects(DateTime start, TimeSpan duration)
        {
            return Intersects(new Interval(start, duration));
        }

        /// <summary>
        /// Determines if this interval and the specified interval are equals. 
        /// </summary>
        /// <param name="other">A <see cref="Interval"/> to compare with this instance.</param>
        /// <returns>
        /// <see langword="true"/> this interval and the specified one are equals. Two intervals
        /// are considered equal if they have exactly the same bounds.
        /// </returns>
        public bool Equals(Interval other)
        {
            if (other == null)
                return false;

            return this.Start == other.Start && this.End == other.End;
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(Thread.CurrentThread.CurrentCulture, "{0}{1}{2}]",
                Start, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator, End);
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;

            return Equals(obj as Interval);
        }

        /// <summary>
        /// Calculates the intersection between two intervals.
        /// </summary>
        /// <param name="value1">First interval, it cannot be an open interval.</param>
        /// <param name="value2">Second interval, it cannot be an open interval.</param>
        /// <returns>
        /// The intersection between the two specified intervals oppure <see cref="Null"/>
        /// if the two intervals do not intersect. Unbounded intervals are not supported then
        /// both input intervals must have both a beginning and an end.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value1"/> is <see langword="null"/>.
        /// <br/>-or-<br/>
        /// If <paramref name="value2"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="value1"/> or <paramref name="value2"/> is open, unbounded intervals are not supported.
        /// </exception>
        public static Interval Intersect(Interval value1, Interval value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");

            if (value2 == null)
                throw new ArgumentNullException("value2");

            if (!value1.Length.HasValue || !value1.Start.HasValue)
                throw new ArgumentException("Cannot intersect an open interval.", "value1");

            if (!value2.Length.HasValue || !value2.Start.HasValue)
                throw new ArgumentException("Cannot intersect an open interval.", "value2");

            DateTime s1 = value1.Start.Value, e1 = value1.End.Value;
            DateTime s2 = value2.Start.Value, e2 = value2.End.Value;

            if (e1 < s2 || s1 > e2)
                return Null;

            if (s1 == s2 && e1 == e2)
                return new Interval(s1, e1);

            DateTime start, end;

            if (s1 <= s2 && e1 <= e2)
            {
                start = s2;
                end = e1;
            }
            else if (s1 <= s2 && e1 >= e2)
            {
                start = s2;
                end = e2;
            }
            else if (s1 >= s2 && e1 <= e2)
            {
                start = s1;
                end = e1;
            }
            else // s1 >= s2 && e1 >= e2
            {
                start = s1;
                end = e2;
            }

            return new Interval(start, end);
        }

        /// <summary>
        /// Compares this interval with a specified timestamp.
        /// </summary>
        /// <param name="timestamp">A timestamp to relate to this interval.</param>
        /// <returns>
        /// -1 if this interval ends before <paramref name="timestamp"/>, 0 if it contains the specified <paramref name="timestamp"/>
        /// or 1 if this interval begins after specified <paramref name="timestamp"/>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)] // Provided for consistency but it is more confusing than normal comparison operators, hidden.
        public int CompareTo(DateTime timestamp)
        {
            if (this < timestamp)
                return -1;
            else if (this > timestamp)
                return 1;

            return 0;
        }

        /// <summary>
        /// Determines if two intervals are equal.
        /// </summary>
        /// <param name="lhs">First interval to compare.</param>
        /// <param name="rhs">Second interval to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="lhs"/> equals to <paramref name="rhs"/>.
        /// </returns>
        public static bool operator ==(Interval lhs, Interval rhs)
        {
            if (Object.ReferenceEquals(lhs, rhs))
                return true;

            if (Object.ReferenceEquals(lhs, null))
                return false;

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines if two intervals are different.
        /// </summary>
        /// <param name="lhs">First interval to compare.</param>
        /// <param name="rhs">Second interval to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="lhs"/> does not equal to <paramref name="rhs"/>.
        /// </returns>
        public static bool operator !=(Interval lhs, Interval rhs)
        {
            if (Object.ReferenceEquals(lhs, rhs))
                return false;

            if (Object.ReferenceEquals(lhs, null))
                return true;

            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines if a specified instant is before the beginning of this interval.
        /// </summary>
        /// <param name="lhs">Instant to compare with the interval.</param>
        /// <param name="rhs">Interval to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="lhs"/> is before the beginning of <paramref name="rhs"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">If <paramref name="rhs"/> is <see cref="Null"/>.</exception>
        public static bool operator <(DateTime lhs, Interval rhs)
        {
            if (rhs == null)
                return false;

            return lhs < rhs.Left(); 
        }

        /// <summary>
        /// Determines if a specified instant is before or at the beginning of this interval.
        /// </summary>
        /// <param name="lhs">Instant to compare with the interval.</param>
        /// <param name="rhs">Interval to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="lhs"/> is before or at the beginning of <paramref name="rhs"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">If <paramref name="rhs"/> is <see cref="Null"/>.</exception>
        public static bool operator <=(DateTime lhs, Interval rhs)
        {
            if (rhs == null)
                return false;

            return lhs <= rhs.Left(); 
        }

        /// <summary>
        /// Determines if a specified instant is after the end of this interval.
        /// </summary>
        /// <param name="lhs">Instant to compare with the interval.</param>
        /// <param name="rhs">Interval to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="lhs"/> is after the end of <paramref name="rhs"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">If <paramref name="rhs"/> is <see cref="Null"/>.</exception>
        public static bool operator >(DateTime lhs, Interval rhs)
        {
            if (rhs == null)
                return true;

            return lhs > rhs.Right(); 
        }

        /// <summary>
        /// Determines if a specified instant is after or at the end of this interval.
        /// </summary>
        /// <param name="lhs">Instant to compare with the interval.</param>
        /// <param name="rhs">Interval to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="lhs"/> is after or at the end of <paramref name="rhs"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">If <paramref name="rhs"/> is <see cref="Null"/>.</exception>
        public static bool operator >=(DateTime lhs, Interval rhs)
        {
            if (rhs == null)
                return true;

            return lhs >= rhs.Right(); 
        }

        /// <summary>
        /// Determines if a specified interval ends before the indicated instant.
        /// </summary>
        /// <param name="lhs">Interval to compare.</param>
        /// <param name="rhs">Instant to compare with the interval.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="rhs"/> is after the end of <paramref name="rhs"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">If <paramref name="lhs"/> is <see cref="Null"/>.</exception>
        public static bool operator <(Interval lhs, DateTime rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
                return true;

            return lhs.Right() < rhs;
        }

        /// <summary>
        /// Determines if a specified interval ends before or at the indicated instant.
        /// </summary>
        /// <param name="lhs">Interval to compare.</param>
        /// <param name="rhs">Instant to compare with the interval.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="rhs"/> is after or at the end of <paramref name="rhs"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">If <paramref name="lhs"/> is <see cref="Null"/>.</exception>
        public static bool operator <=(Interval lhs, DateTime rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
                return true;

            return lhs.Right() <= rhs;
        }

        /// <summary>
        /// Determines if a specified interval begins after the indicated instant.
        /// </summary>
        /// <param name="lhs">Interval to compare.</param>
        /// <param name="rhs">Instant to compare with the interval.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="rhs"/> is before the beginning of <paramref name="rhs"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">If <paramref name="lhs"/> is <see cref="Null"/>.</exception>
        public static bool operator >(Interval lhs, DateTime rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
                return false;

            return lhs.Left() > rhs;
        }

        /// <summary>
        /// Determines if a specified interval begins after or at the indicated instant.
        /// </summary>
        /// <param name="lhs">Interval to compare.</param>
        /// <param name="rhs">Instant to compare with the interval.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="rhs"/> is before or at the beginning of <paramref name="rhs"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">If <paramref name="lhs"/> is <see cref="Null"/>.</exception>
        public static bool operator >=(Interval lhs, DateTime rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
                return false;

            return lhs.Left() >= rhs;
        }

        private readonly DateTime? _start, _end;

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "dummy", Justification = "Dummy parameter to introduce a constructor without range checking")]
        private Interval(DateTime? start, DateTime? end, int dummy)
        {
            _start = start;
            _end = end;
        }

        private DateTime Left()
        {
            if (Start.HasValue)
                return Start.Value;

            return End.Value;
        }

        private DateTime Right()
        {
            if (End.HasValue)
                return End.Value;

            return Start.Value;
        }
    }
}
