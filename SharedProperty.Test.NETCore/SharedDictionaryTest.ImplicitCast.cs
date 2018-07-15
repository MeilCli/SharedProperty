using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedProperty.NETStandard;

namespace SharedProperty.Test.NETCore
{
    public partial class SharedDictionaryTest
    {
        private const string key = "key";

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestUpcast()
        {
            var sharedDictionary = new SharedDictionary(EmptySerializer.Default, null, null);
            sharedDictionary.SetProperty(key, "value");
            sharedDictionary.GetProperty<object>(key);
        }

        interface ICovariance<out T> { }

        class Covariance<T> : ICovariance<T> { }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestCovariance()
        {
            var sharedDictionary = new SharedDictionary(EmptySerializer.Default, null, null);
            sharedDictionary.SetProperty(key, new Covariance<string>());
            sharedDictionary.GetProperty<ICovariance<object>>(key);
        }

        interface IContravariance<in T> { }

        class Contravariance<T> : IContravariance<T> { }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestContravariance()
        {
            var sharedDictionary = new SharedDictionary(EmptySerializer.Default, null, null);
            sharedDictionary.SetProperty(key, new Contravariance<object>());
            sharedDictionary.GetProperty<IContravariance<string>>(key);
        }

        struct NullableData { }

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestNullable()
        {
            var sharedDictionary = new SharedDictionary(EmptySerializer.Default, null, null);
            sharedDictionary.SetProperty<NullableData>(key, new NullableData());
            sharedDictionary.GetProperty<NullableData?>(key);
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

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestImplicitOperator1()
        {
            var sharedDictionary = new SharedDictionary(EmptySerializer.Default, null, null);
            sharedDictionary.SetProperty(key, new ImplicitOperatableSource1());
            sharedDictionary.GetProperty<ImplicitOperatableTarget1>(key);
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

        [TestCategory(TestCategoryConstant.ImplicitCast)]
        [TestMethod]
        public void TestImplicitOperator2()
        {
            var sharedDictionary = new SharedDictionary(EmptySerializer.Default, null, null);
            sharedDictionary.SetProperty(key, new ImplicitOperatableSource2());
            sharedDictionary.GetProperty<ImplicitOperatableTarget2>(key);
        }
    }
}
