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
using System.Globalization;
using System.Text;

namespace Bsa.Design
{
    /// <summary>
    /// Converts <see cref="Interval"/> to other representations.
    /// </summary>
    public sealed class IntervalTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return "";

            string text = value.ToString();
            if (text.Length == 0)
                return Interval.Null;

            string[] parts = text.Split(new char[] { ';' }, 2);
            DateTime? from = null, to = null;

            if (!String.IsNullOrWhiteSpace(parts[0]))
            {
                from = DateTime.ParseExact(parts[0], "yyyy/MM/dd HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
            }

            if (!String.IsNullOrWhiteSpace(parts[1]))
            {
                to = DateTime.ParseExact(parts[1], "yyyy/MM/dd HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture);
            }

            return new Interval(from, to);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
                return base.ConvertTo(context, culture, value, destinationType);

            var interval = (Interval)value;
            var result = new StringBuilder();

            if (interval.Start.HasValue)
            {
                result.Append(interval.Start.Value.ToString("yyyy/MM/dd HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture));
            }

            result.Append(';');

            if (interval.End.HasValue)
            {
                result.Append(interval.End.Value.ToString("yyyy/MM/dd HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture));
            }

            return result.ToString();
        }
    }
}
