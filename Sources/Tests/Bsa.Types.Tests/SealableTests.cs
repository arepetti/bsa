using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bsa.Tests
{
    [TestClass]
    public class SealableTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenSealedIsImmutable()
        {
            var test = new SealableTest();
            
            test.Seal();            
            Assert.IsTrue(test.IsSealed);

            test.Value = 1;
        }

        [TestMethod]
        public void CanClone()
        {
            var test = new SealableTest();
            
            test.Seal();
            Assert.IsTrue(test.IsSealed);

            var clone = test.Clone<SealableTest>();
            Assert.IsFalse(clone.IsSealed);
            
            clone.Value = 1;
            Assert.AreEqual(1, clone.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void CloneTypeMustMatch1()
        {
            new SealableTest().Clone<double>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CloneTypeMustMatch2()
        {
            new FaultyCloneable().Clone<FaultyCloneable>();
        }

        sealed class FaultyCloneable : Sealable
        {
            protected override Sealable CreateNewInstance()
            {
                // Wrong on purpose: caller expects FaultyCloneable
                return new SealableTest();
            }

            protected override void CopyPropertiesTo(Sealable target)
            {
            }
        }

        sealed class SealableTest : Sealable
        {
            public int Value
            {
                get { return _value; }
                set
                {
                    ThrowIfSealed();
                    _value = value;
                }
            }

            protected override Sealable CreateNewInstance()
            {
                return new SealableTest();
            }

            protected override void CopyPropertiesTo(Sealable target)
            {
                ((SealableTest)target).Value = this.Value;
            }

            private int _value;
        }
    }
}
