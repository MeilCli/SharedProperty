using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedProperty.NETStandard;

namespace SharedProperty.Test.NETCore.TypeConverters
{
    [TestClass]
    public class FloatTypeConvertTest
    {
        private const string key = "key";
        private static readonly ISharedDictionary sharedDictionary
            = new SharedDictionary(EmptySerializer.Default, null, null);

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            sharedDictionary.SetProperty<float>(key, 1);
        }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestConvertDouble()
        {
            Assert.IsInstanceOfType(sharedDictionary.GetProperty<double>(key), typeof(double));
        }
    }
}
