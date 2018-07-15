namespace SharedProperty.Test.NETCore.Serializers
{
    internal class JsonConstant
    {
        public const string ShortModeJson =
            "{" +
            "\"version\":\"1.0\"," +
            "\"property_type\":\"short\"," +
            "\"properties\":{" +
            "\"key1\":{" +
            "\"System.String, System.Private.CoreLib\":\"string\"" +
            "}," +
            "\"key2\":{" +
            "\"System.Int32, System.Private.CoreLib\":1" +
            "}" +
            "}" +
            "}";

        public const string LargeModeJson =
            "{" +
            "\"version\":\"1.0\"," +
            "\"property_type\":\"large\"," +
            "\"properties\":[" +
            "{" +
            "\"key\":\"key1\"," +
            "\"type\":\"System.String, System.Private.CoreLib\"," +
            "\"value\":\"string\"" +
            "}," +
            "{" +
            "\"key\":\"key2\"," +
            "\"type\":\"System.Int32, System.Private.CoreLib\"," +
            "\"value\":1" +
            "}" +
            "]" +
            "}";

        public const string ShortModeJsonWithUnknownData =
            "{" +
            "\"version\":\"1.0\"," +
            "\"property_type\":\"short\"," +
            "\"properties\":{" +
            "\"key1\":{" +
            "\"SharedProperty.Test.NETCore.Serializers.UnknownData, SharedProperty.Test.NETCore\":\"string\"" +
            "}," +
            "\"key2\":{" +
            "\"System.Int32, System.Private.CoreLib\":1" +
            "}" +
            "}" +
            "}";

        public const string LargeModeJsonWithUnknownData =
            "{" +
            "\"version\":\"1.0\"," +
            "\"property_type\":\"large\"," +
            "\"properties\":[" +
            "{" +
            "\"key\":\"key1\"," +
            "\"type\":\"SharedProperty.Test.NETCore.Serializers.UnknownData, SharedProperty.Test.NETCore\"," +
            "\"value\":\"string\"" +
            "}," +
            "{" +
            "\"key\":\"key2\"," +
            "\"type\":\"System.Int32, System.Private.CoreLib\"," +
            "\"value\":1" +
            "}" +
            "]" +
            "}";
    }
}
