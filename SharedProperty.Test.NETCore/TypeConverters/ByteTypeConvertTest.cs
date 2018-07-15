using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedProperty.NETStandard;

namespace SharedProperty.Test.NETCore.TypeConverters
{
    [TestClass]
    public class ByteTypeConvertTest
    {
        private const string key = "key";
        private static readonly ISharedDictionary sharedDictionary
            = new SharedDictionary(EmptySerializer.Default, null, null);

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            sharedDictionary.SetProperty<byte>(key, 1);
        }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestConvertShort()
        {
            Assert.IsInstanceOfType(sharedDictionary.GetProperty<short>(key), typeof(short));
        }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestConvertUnsignedShort()
        {
            Assert.IsInstanceOfType(sharedDictionary.GetProperty<ushort>(key), typeof(ushort));
        }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestConvertInt()
        {
            Assert.IsInstanceOfType(sharedDictionary.GetProperty<int>(key), typeof(int));
        }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestConvertUnsignedInt()
        {
            Assert.IsInstanceOfType(sharedDictionary.GetProperty<uint>(key), typeof(uint));
        }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestConvertLong()
        {
            Assert.IsInstanceOfType(sharedDictionary.GetProperty<long>(key), typeof(long));
        }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestConvertUnsignedLong()
        {
            Assert.IsInstanceOfType(sharedDictionary.GetProperty<ulong>(key), typeof(ulong));
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
