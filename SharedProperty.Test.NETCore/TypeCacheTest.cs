using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedProperty.NETStandard;
using System;
using System.Collections.Generic;

namespace SharedProperty.Test.NETCore
{
    [TestClass]
    public class TypeCacheTest
    {
        public class Inner
        {
            public class InnerOfInner { }
        }

        public class InnerGenerics<T>
        {
            public class InnerGenericsOfInnerGenerics<R> { }
        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestNormal()
        {
            Assert.AreEqual(Type.GetType(TypeCache<TypeCacheTest>.FullName), typeof(TypeCacheTest));
        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestDifferentAssembly()
        {
            Assert.AreEqual(Type.GetType(TypeCache<SharedDictionary>.FullName), typeof(SharedDictionary));
        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestGenerics()
        {
            Assert.AreEqual(Type.GetType(TypeCache<List<string>>.FullName), typeof(List<string>));
        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestMultiGenerics()
        {
            Assert.AreEqual(Type.GetType(TypeCache<Dictionary<string, string>>.FullName), typeof(Dictionary<string, string>));
        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestGenericsOfGenerics()
        {
            Assert.AreEqual(Type.GetType(TypeCache<List<List<string>>>.FullName), typeof(List<List<string>>));
        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestInner()
        {
            Assert.AreEqual(Type.GetType(TypeCache<Inner>.FullName), typeof(Inner));
        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestInnerOfInner()
        {
            Assert.AreEqual(Type.GetType(TypeCache<Inner.InnerOfInner>.FullName), typeof(Inner.InnerOfInner));
        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestInnerGenerics()
        {
            Assert.AreEqual(Type.GetType(TypeCache<InnerGenerics<string>>.FullName), typeof(InnerGenerics<string>));
        }

        [TestCategory(TestCategoryConstant.Type)]
        [TestMethod]
        public void TestInnerGenericsOfInnerGenerics()
        {
            Assert.AreEqual(Type.GetType(TypeCache<InnerGenerics<string>.InnerGenericsOfInnerGenerics<string>>.FullName),
                typeof(InnerGenerics<string>.InnerGenericsOfInnerGenerics<string>));
        }
    }
}
