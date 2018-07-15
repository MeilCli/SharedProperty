using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedProperty.NETStandard.Extensions;

namespace SharedProperty.Test.NETCore.Extensions
{
    [TestClass]
    public class TypeExtensionTest
    {
        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestNotHasImplicitOperator()
        {
            Assert.AreEqual(typeof(TypeExtensionTest).CanImplicitOperatingConvert(typeof(string)), false);
        }

        class ImplicitOperatableSource1
        {
            public static implicit operator ImplicitOperatableTarget1(ImplicitOperatableSource1 source)
            {
                return new ImplicitOperatableTarget1();
            }
        }

        class ImplicitOperatableTarget1
        {

        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestCanImplicitOperatingConvert1()
        {
            Assert.AreEqual(
                typeof(ImplicitOperatableSource1).CanImplicitOperatingConvert(typeof(ImplicitOperatableTarget1)),
                true
            );
        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestNotHasImplicitOperator1()
        {
            Assert.AreEqual(
                typeof(ImplicitOperatableTarget1).CanImplicitOperatingConvert(typeof(ImplicitOperatableSource1)),
                false
            );
        }

        class ImplicitOperatableSource2
        {

        }

        class ImplicitOperatableTarget2
        {
            public static implicit operator ImplicitOperatableTarget2(ImplicitOperatableSource2 source)
            {
                return new ImplicitOperatableTarget2();
            }
        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestCanImplicitOperatingConvert2()
        {
            Assert.AreEqual(
                typeof(ImplicitOperatableSource2).CanImplicitOperatingConvert(typeof(ImplicitOperatableTarget2)),
                true
            );
        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestNotHasImplicitOperator2()
        {
            Assert.AreEqual(
                typeof(ImplicitOperatableTarget2).CanImplicitOperatingConvert(typeof(ImplicitOperatableSource2)),
                false
            );
        }
    }
}
