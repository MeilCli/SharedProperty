using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedProperty.NETStandard;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProperty.Test.NETCore.TypeConverters
{
    [TestClass]
    public class IntTypeConvertTest
    {
        private const string key = "key";
        private static readonly ISharedDictionary sharedDictionary
            = new SharedDictionary(EmptySerializer.Default, null, null);

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            sharedDictionary.SetProperty<int>(key, 1);
        }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestConvertLong()
        {
            Assert.IsInstanceOfType(sharedDictionary.GetProperty<long>(key), typeof(long));
        }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestConvertFloat()
        {
            Assert.IsInstanceOfType(sharedDictionary.GetProperty<float>(key), typeof(float));
        }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestConvertDouble()
        {
            Assert.IsInstanceOfType(sharedDictionary.GetProperty<double>(key), typeof(double));
        }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestConvertDecimal()
        {
            Assert.IsInstanceOfType(sharedDictionary.GetProperty<decimal>(key), typeof(decimal));
        }
    }
}
