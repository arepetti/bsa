//
// MoreLINQ - Extensions to LINQ to Objects
// Copyright (c) 2008 Jonathan Skeet. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// This implementation is based on original implementation (with above
// copyright notice). Original implementation is available
// at: https://github.com/morelinq/MoreLINQ/blob/master/MoreLinq/DistinctBy.cs
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bsa
{
    /// <summary>
    /// LINQ helper methods.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Returns all distinct elements in specified enumeration projecting
        /// elements with given function and using given equality comparer.
        /// </summary>
        /// <typeparam name="TSource">Type of the source enumeration.</typeparam>
        /// <typeparam name="TKey">Type of the projected element.</typeparam>
        /// <param name="source">Source enumeration to scan for distinct elements.</param>
        /// <param name="keySelector">
        /// Function used to project <paramref name="source"/> elements into <typeparamref name="TKey"/>
        /// elements.
        /// </param>
        /// <param name="comparer">
        /// Equality comparer used to determine distinctness. If omitted (<see langword="null"/>)
        /// then default equality comparer is used.
        /// </param>
        /// <returns>
        /// All distinct (according to projected <typeparamref name="TKey"/> via
        /// <paramref name="keySelector"/> function) elements, compared with given equality comparer.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="source"/> is <see langword="null"/>.
        /// <br/>-or-<br/>
        /// If <paramref name="keySelector"/> is <see langword="null"/>.
        /// </exception>
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (keySelector == null)
                throw new ArgumentNullException("keySelector");

            return DistinctCore(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> DistinctCore<TSource, TKey>(IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var keys = new HashSet<TKey>(comparer);

            foreach (var element in source)
            {
                if (keys.Add(keySelector(element)))
                    yield return element;
            }
        }
    }
}