using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MutationTracker.Test
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void TestThis()
        {
            var o = new TestObject
            {
                ShortNullField = 5,
                BoolField = true,
                DecimalProperty = 1,

                StructField = new TestObject.TestStruct
                {
                    IntField = 5,
                    DecimalField = 1.5m
                }
            };

            Assert.IsFalse(o.IsModified);

            o.Track();
            Assert.IsFalse(o.IsModified);

            o.ShortNullField = null;
            Assert.IsTrue(o.IsModified);
            o.ShortNullField = 4;
            Assert.IsTrue(o.IsModified);
            o.ShortNullField = 5;
            Assert.IsFalse(o.IsModified);

            o.BoolField = false;
            Assert.IsTrue(o.IsModified);
            o.BoolField = true;
            Assert.IsFalse(o.IsModified);

            o.DecimalProperty = 4;
            Assert.IsTrue(o.IsModified);
            o.DecimalProperty = 1;
            Assert.IsFalse(o.IsModified);

            o.StructField.IntField = 4;
            Assert.IsTrue(o.IsModified);
            o.StructField.IntField = 5;
            Assert.IsFalse(o.IsModified);

            o.StructField.DecimalField = 1.4m;
            Assert.IsTrue(o.IsModified);
            o.StructField.DecimalField = 0;
            Assert.IsTrue(o.IsModified);
            o.StructField.DecimalField = 1.5m;
            Assert.IsFalse(o.IsModified);
        }

        [TestMethod]
        public void TestBase()
        {
            var o = new TestObject
            {
                IntPropertyBase = 5,
                StringFieldBase = null,
            };

            o.IntPropertyBase = 2 + 2;
            Assert.IsFalse(o.IsModified); // private [backing] fields of the base class are not tracked
            o.IntPropertyBase = 2 + 3;
            Assert.IsFalse(o.IsModified);

            o.StringFieldBase = "AS";
            Assert.IsTrue(o.IsModified);
            o.StringFieldBase = null;
            Assert.IsFalse(o.IsModified);
        }
    }


    internal class BaseTestObject
    {
        public int IntPropertyBase { get; set; }

        public string StringFieldBase;
    }

    internal class TestObject : BaseTestObject
    {
        public short? ShortNullField;

        public bool BoolField;

        public decimal DecimalProperty { get; set; }

        public TestStruct StructField;

        public struct TestStruct : IEquatable<TestStruct>
        {
            public int IntField;
            public decimal DecimalField;

            public bool Equals(TestStruct other)
            {
                return
                    this.IntField     == other.IntField  &&
                    this.DecimalField == other.DecimalField;
            }
        }

        #region Tracking

        private Func<bool> isModified = () => false;

        public bool IsModified => this.isModified();
        public void Track() => this.isModified = this.TrackFields();

        #endregion
    }
}