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
// along with BSA-F.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.ObjectModel;

namespace Bsa
{
    /// <summary>
    /// Represents a collection of <see cref="Sealable"/> objects.
    /// </summary>
    public abstract class SealableCollection<T> : Collection<T>, ISealable
        where T : Sealable
    {
        /// <summary>
        /// Indicates whether this object has been sealed.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this object has been sealed and it then cannot be further modified.
        /// </value>
        public bool IsSealed
        {
            get { return _isSealed; }
        }

        /// <summary>
        /// Seal this object preventing any further modifcation.
        /// </summary>
        /// <remarks>
        /// Changing an object after it has been sealed will throw <see cref="InvalidOperationException"/>. Sealing
        /// this collection will also seal all contained items.
        /// </remarks>
        public void Seal()
        {
            _isSealed = true;
            foreach (var channel in Items)
                channel.Seal();
        }

        protected override void InsertItem(int index, T item)
        {
            ThrowIfSealed();
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            ThrowIfSealed();
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            ThrowIfSealed();
            base.SetItem(index, item);
        }

        protected override void ClearItems()
        {
            ThrowIfSealed();
            base.ClearItems();
        }

        private bool _isSealed;

        private void ThrowIfSealed()
        {
            if (IsSealed)
                throw new InvalidOperationException("This object has been finalized and cannot be modified.");
        }
    }
}
