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

namespace Bsa
{
    /// <summary>
    /// Represents an object that can be <em>finalized</em> and made immutable, changes must be applied
    /// creating a deep copy and updating the copy.
    /// </summary>
    /// <remarks>
    /// To implement deep-clone without knowing here the exact type this class splits cloning in two methods:
    /// one <see cref="CreateNewInstance"/> to create a new instance (overridden by each non-abstract derived class)
    /// and one to effectively copy properties from each class to the target instance. Obviously <see cref="IsSealed"/>
    /// property isn't propagated. Failing to implement one of those methods when required will cause these issues:
    /// wrong object will be instantiated (causing a <see cref="InvalidCastException"/> in <see cref="CopyPropertiesTo"/>
    /// if correctly implemented) or non updated properties (if <see cref="CopyPropertiesTo"/> is not overridden in a
    /// derived class). It is reccomended to always setup tests for cloning because some properties may be cause
    /// subtle bugs or unexpected operations. If all objects in the hierarchy are <c>[Serializable]</c> in tests you may
    /// compare them using binary serialization (to avoid to compare each property).
    /// <br/>
    /// Derived classes should always call <see cref="ThrowIfSealed"/> before changing object state (also in response
    /// to caller's actions), it will throw <see cref="InvalidOperationException"/> if object has been previously
    /// sealed calling <see cref="Seal"/>.
    /// </remarks>
    [Serializable]
    public abstract class Sealable : ISealable, ICloneable
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
        /// Changing an object after it has been sealed will throw <see cref="InvalidOperationException"/>. You can
        /// use <see cref="IsSealed"/> to determine if an object has been sealed and you should clone it with
        /// <see cref="Clone{T}"/> to apply changes.
        /// </remarks>
        public virtual void Seal()
        {
            _isSealed = true;
        }

        /// <summary>
        /// Creates a new instance of this class which is a clone of this object.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object you want to clone, it can be the effective type that will be created
        /// or a less-derived class in the hierarchy.
        /// </typeparam>
        /// <returns>
        /// A new instance of this object where all properties are (deep) copied from this one the the
        /// clone. Effective object must be at least <typeparamref name="T"/> or one of its derived classes.
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// If <typeparamref name="T"/> is not a type which belongs to this object classes hierarchy.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// There is a programming error and most derived class failed to provide a meaningful implementation
        /// of <see cref="CreateNewInstance"/> method.
        /// </exception>
        public T Clone<T>()
        {
            return (T)((ICloneable)this).Clone();
        }

        /// <summary>
        /// Creates a new instance of this class which is a clone of this object.
        /// </summary>
        /// <returns>
        /// A new instance of this object where all properties are (deep) copied from this one the clone.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// There is a programming error and most derived class failed to provide a meaningful implementation
        /// of <see cref="CreateNewInstance"/> method.
        /// </exception>
        object ICloneable.Clone()
        {
            var clone = CreateNewInstance();
            if (clone == null || clone.GetType() != GetType())
                throw new InvalidOperationException(String.Format("Derived class created the wrong object instance, expected: {0}, actual: {1}", GetType().Name, clone.GetType().Name));

            CopyPropertiesTo(clone);

            return clone;
        }

        /// <summary>
        /// Creates a new empty instance of this object.
        /// </summary>
        /// <returns>
        /// A new instance of this object, each derived non-abstract class must override this method to create
        /// an instance of the right type to support deep-clooning. Abstract classes may override this method
        /// to throw <see cref="NotImplementedException"/>. Derived classes must not call base class implementation.
        /// </returns>
        /// <seealso cref="CopyPropertiesTo"/>
        protected abstract Sealable CreateNewInstance();

        /// <summary>
        /// Copy all the properties and state from this object to the specified target.
        /// </summary>
        /// <param name="target">Object to which properties must be copied to.</param>
        /// <remarks>
        /// Each derived class must copy only properties in that class (casting <paramref name="target"/>
        /// parameter to proper type, do not ever assume a more specialized type).
        /// </remarks>
        /// <seealso cref="CreateNewInstance"/>
        protected abstract void CopyPropertiesTo(Sealable target);

        /// <summary>
        /// Throw <see cref="InvalidOperationException"/> if <see cref="IsSealed"/> is <see langword="true"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="IsSealed"/> is <see langword="true"/>.
        /// </exception>
        protected void ThrowIfSealed()
        {
            if (IsSealed)
                throw new InvalidOperationException("This object has been finalized and cannot be modified.");
        }

        [NonSerialized]
        private bool _isSealed;
    }
}
