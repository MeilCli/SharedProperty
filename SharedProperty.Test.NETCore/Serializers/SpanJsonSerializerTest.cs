using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedProperty.NETStandard;
using SharedProperty.Serializer.SpanJson;
using System.Text;

namespace SharedProperty.Test.NETCore.Serializers
{
    [TestClass]
    public class SpanJsonSerializerTest
    {
        [TestCategory(TestCategoryConstant.Serializee)]
        [TestMethod]
        public void ShortModeSerialize()
        {
            var sharedDictionary = new SharedDictionary(SpanJsonSerializer.Create(SerializeMode.ShortObject), null, null);
            sharedDictionary.RawImport(Encoding.UTF8.GetBytes(JsonConstant.ShortModeJson));
            Assert.AreEqual(sharedDictionary.GetProperty<string>("key1"), "string");
            Assert.AreEqual(sharedDictionary.GetProperty<int>("key2"), 1);
        }

        [TestCategory(TestCategoryConstant.Serializee)]
        [TestMethod]
        public void LargeModeSerialize()
        {
            var sharedDictionary = new SharedDictionary(SpanJsonSerializer.Create(SerializeMode.LargeObject), null, null);
            sharedDictionary.RawImport(Encoding.UTF8.GetBytes(JsonConstant.LargeModeJson));
            Assert.AreEqual(sharedDictionary.GetProperty<string>("key1"), "string");
            Assert.AreEqual(sharedDictionary.GetProperty<int>("key2"), 1);
        }

        [TestCategory(TestCategoryConstant.Serializee)]
        [TestMethod]
        public void ShortModeDeserialize()
        {
            var sharedDictionary = new SharedDictionary(SpanJsonSerializer.Create(SerializeMode.ShortObject), null, null);
            sharedDictionary.SetProperty("key1", "string");
            sharedDictionary.SetProperty("key2", 1);
            byte[] bytes = sharedDictionary.RawExport();
            Assert.AreEqual(Encoding.UTF8.GetString(bytes), JsonConstant.ShortModeJson);
        }

        [TestCategory(TestCategoryConstant.Serializee)]
        [TestMethod]
        public void LargeModeDeserialize()
        {
            var sharedDictionary = new SharedDictionary(SpanJsonSerializer.Create(SerializeMode.LargeObject), null, null);
            sharedDictionary.SetProperty("key1", "string");
            sharedDictionary.SetProperty("key2", 1);
            byte[] bytes = sharedDictionary.RawExport();
            Assert.AreEqual(Encoding.UTF8.GetString(bytes), JsonConstant.LargeModeJson);
        }

        [TestCategory(TestCategoryConstant.Serializee)]
        [TestMethod]
        public void ShortModeSerializeWithUnknownData()
        {
            var sharedDictionary = new SharedDictionary(SpanJsonSerializer.Create(SerializeMode.ShortObject), null, null);
            sharedDictionary.RawImport(Encoding.UTF8.GetBytes(JsonConstant.ShortModeJsonWithUnknownData));
            Assert.AreEqual(sharedDictionary.PropertyCount, 1);
            Assert.AreEqual(sharedDictionary.GetProperty<int>("key2"), 1);
        }

        [TestCategory(TestCategoryConstant.Serializee)]
        [TestMethod]
        public void LargeModeSerializeWithUnknownData()
        {
            var sharedDictionary = new SharedDictionary(SpanJsonSerializer.Create(SerializeMode.LargeObject), null, null);
            sharedDictionary.RawImport(Encoding.UTF8.GetBytes(JsonConstant.LargeModeJsonWithUnknownData));
            Assert.AreEqual(sharedDictionary.PropertyCount, 1);
            Assert.AreEqual(sharedDictionary.GetProperty<int>("key2"), 1);
        }
    }
}
