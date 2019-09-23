using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedProperty.NETStandard;
using SharedProperty.Serializer.SystemTextJson;

namespace SharedProperty.Test.NETCore.Serializers
{
    [TestClass]
    public class SystemTextJsonSerializerTest
    {
        [TestCategory(TestCategoryConstant.Serializer)]
        [TestMethod]
        public void ShortModeSerialize()
        {
            var sharedDictionary = new SharedDictionary(new SystemTextJsonSerializer(SerializeMode.ShortObject), null, null);
            sharedDictionary.RawImport(Encoding.UTF8.GetBytes(JsonConstant.ShortModeJson));
            Assert.AreEqual(sharedDictionary.GetProperty<string>("key1"), "string");
            Assert.AreEqual(sharedDictionary.GetProperty<int>("key2"), 1);
        }

        [TestCategory(TestCategoryConstant.Serializer)]
        [TestMethod]
        public void LargeModeSerialize()
        {
            var sharedDictionary = new SharedDictionary(new SystemTextJsonSerializer(SerializeMode.LargeObject), null, null);
            sharedDictionary.RawImport(Encoding.UTF8.GetBytes(JsonConstant.LargeModeJson));
            Assert.AreEqual(sharedDictionary.GetProperty<string>("key1"), "string");
            Assert.AreEqual(sharedDictionary.GetProperty<int>("key2"), 1);
        }

        [TestCategory(TestCategoryConstant.Serializer)]
        [TestMethod]
        public void ShortModeDeserialize()
        {
            var sharedDictionary = new SharedDictionary(new SystemTextJsonSerializer(SerializeMode.ShortObject), null, null);
            sharedDictionary.SetProperty("key1", "string");
            sharedDictionary.SetProperty("key2", 1);
            byte[] bytes = sharedDictionary.RawExport();
            Assert.AreEqual(Encoding.UTF8.GetString(bytes), JsonConstant.ShortModeJson);
        }

        [TestCategory(TestCategoryConstant.Serializer)]
        [TestMethod]
        public void LargeModeDeserialize()
        {
            var sharedDictionary = new SharedDictionary(new SystemTextJsonSerializer(SerializeMode.LargeObject), null, null);
            sharedDictionary.SetProperty("key1", "string");
            sharedDictionary.SetProperty("key2", 1);
            byte[] bytes = sharedDictionary.RawExport();
            Assert.AreEqual(Encoding.UTF8.GetString(bytes), JsonConstant.LargeModeJson);
        }

        [TestCategory(TestCategoryConstant.Serializer)]
        [TestMethod]
        public void ShortModeSerializeWithUnknownData()
        {
            var sharedDictionary = new SharedDictionary(new SystemTextJsonSerializer(SerializeMode.ShortObject), null, null);
            sharedDictionary.RawImport(Encoding.UTF8.GetBytes(JsonConstant.ShortModeJsonWithUnknownData));
            Assert.AreEqual(sharedDictionary.PropertyCount, 1);
            Assert.AreEqual(sharedDictionary.GetProperty<int>("key2"), 1);
        }

        [TestCategory(TestCategoryConstant.Serializer)]
        [TestMethod]
        public void LargeModeSerializeWithUnknownData()
        {
            var sharedDictionary = new SharedDictionary(new SystemTextJsonSerializer(SerializeMode.LargeObject), null, null);
            sharedDictionary.RawImport(Encoding.UTF8.GetBytes(JsonConstant.LargeModeJsonWithUnknownData));
            Assert.AreEqual(sharedDictionary.PropertyCount, 1);
            Assert.AreEqual(sharedDictionary.GetProperty<int>("key2"), 1);
        }

        [TestCategory(TestCategoryConstant.Serializer)]
        [TestMethod]
        public void ShortModeNullValue()
        {
            var sharedDictionary = new SharedDictionary(new SystemTextJsonSerializer(SerializeMode.ShortObject), null, null);
            sharedDictionary.SetProperty<int?>("nullableInt", null);
            sharedDictionary.SetProperty<string?>("nullableString", null);

            byte[] binary = sharedDictionary.RawExport();
            sharedDictionary.ClearProperty();
            Assert.AreEqual(0, sharedDictionary.PropertyCount);
            sharedDictionary.RawImport(binary);

            Assert.AreEqual(null, sharedDictionary.GetProperty<int?>("nullableInt"));
            Assert.AreEqual(null, sharedDictionary.GetProperty<string?>("nullableString"));
        }

        [TestCategory(TestCategoryConstant.Serializer)]
        [TestMethod]
        public void LargeModeNullValue()
        {
            var sharedDictionary = new SharedDictionary(new SystemTextJsonSerializer(SerializeMode.LargeObject), null, null);
            sharedDictionary.SetProperty<int?>("nullableInt", null);
            sharedDictionary.SetProperty<string?>("nullableString", null);

            byte[] binary = sharedDictionary.RawExport();
            sharedDictionary.ClearProperty();
            Assert.AreEqual(0, sharedDictionary.PropertyCount);
            sharedDictionary.RawImport(binary);

            Assert.AreEqual(null, sharedDictionary.GetProperty<int?>("nullableInt"));
            Assert.AreEqual(null, sharedDictionary.GetProperty<string?>("nullableString"));
        }
    }
}
