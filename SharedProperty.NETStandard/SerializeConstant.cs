using System;

namespace SharedProperty.NETStandard
{
    public static class SerializeConstant
    {
        public const string VersionName = "version";
        private const string version = "1.0";
        public static readonly Version Version = new Version(version);

        public const string PropertyTypeName = "property_type";
        public const string PropertyTypeLargeValue = "large";
        public const string PropertyTypeShortValue = "short";

        public const string PropertiesName = "properties";
        public const string KeyName = "key";
        public const string TypeName = "type";
        public const string ValueName = "value";
    }
}
