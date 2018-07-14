using SharedProperty.NETStandard;
using System;
using System.Collections.Generic;
using Utf8Json;
using Utf8Json.Formatters;

namespace SharedProperty.Serializer.Utf8Json
{
    public class Utf8JsonSerializer : ISerializer
    {
        private static class MemoryPool
        {
            [ThreadStatic]
            private static byte[] buffer = null;

            public static byte[] GetBuffer()
            {
                buffer = buffer ?? new byte[short.MaxValue];
                return buffer;
            }
        }

        public static readonly Utf8JsonSerializer Default = new Utf8JsonSerializer();

        private readonly Utf8JsonFormatterResolver utf8JsonFormatterResolver;

        public IFormatterResolver FormatterResolver {
            get => utf8JsonFormatterResolver;
        }

        public SerializeMode SerializeMode { get; }

        public Utf8JsonSerializer()
            : this(new Utf8JsonFormatterResolver()) { }

        public Utf8JsonSerializer(Utf8JsonFormatterResolver jsonFormatterResolver)
            : this(jsonFormatterResolver, SerializeMode.ShortObject) { }

        public Utf8JsonSerializer(SerializeMode serializeMode)
            : this(new Utf8JsonFormatterResolver(), serializeMode) { }

        public Utf8JsonSerializer(Utf8JsonFormatterResolver jsonFormatterResolver, SerializeMode serializeMode)
        {
            this.utf8JsonFormatterResolver = jsonFormatterResolver;
            SerializeMode = serializeMode;
        }

        public IEnumerable<IProperty> Deserialize(byte[] binary)
        {
            var reader = new JsonReader(binary);
            reader.ReadIsBeginObjectWithVerify();
            int count = 0;
            string propertyType = null;
            IEnumerable<IProperty> result = null;
            while (reader.ReadIsEndObjectWithSkipValueSeparator(ref count) == false)
            {
                string name = reader.ReadPropertyName();
                switch (name)
                {
                    case SerializeConstant.VersionName:
                        reader.ReadString();
                        break;
                    case SerializeConstant.PropertyTypeName:
                        propertyType = reader.ReadString();
                        break;
                    case SerializeConstant.PropertiesName:
                        result = readProperiesValue(ref reader, propertyType);
                        break;
                }
            }
            return result ?? throw new InvalidOperationException("not found properties");
        }

        private IEnumerable<IProperty> readProperiesValue(ref JsonReader reader, string propertyType)
        {
            if (propertyType == null)
            {
                throw new InvalidOperationException("properties must be last json order");
            }

            switch (propertyType)
            {
                case SerializeConstant.PropertyTypeLargeValue:
                    return readPropertiesValueWithLargeType(ref reader);
                case SerializeConstant.PropertyTypeShortValue:
                    return readPropertiesValueWithShortType(ref reader);
            }
            return null;
        }

        private IEnumerable<IProperty> readPropertiesValueWithLargeType(ref JsonReader reader)
        {
            var result = new List<IProperty>();
            reader.ReadIsBeginArrayWithVerify();
            int arrayCount = 0;
            while (reader.ReadIsEndArrayWithSkipValueSeparator(ref arrayCount) == false)
            {
                string key = null;
                string type = null;
                IProperty property = null;
                IUtf8JsonFormatter formatter = null;
                reader.ReadIsBeginObjectWithVerify();
                int objectCount = 0;
                while (reader.ReadIsEndObjectWithSkipValueSeparator(ref objectCount) == false)
                {
                    string name = reader.ReadPropertyName();
                    switch (name)
                    {
                        case SerializeConstant.KeyName:
                            key = reader.ReadString();
                            break;
                        case SerializeConstant.TypeName:
                            type = reader.ReadString();
                            break;
                        case SerializeConstant.ValueName:
                            if (type == null)
                            {
                                throw new InvalidOperationException("value must be last json order");
                            }

                            formatter = utf8JsonFormatterResolver.Resolve(type);
                            if (formatter == null)
                            {
                                PrimitiveObjectFormatter.Default.Deserialize(ref reader, utf8JsonFormatterResolver.JsonFormatterResolver);
                                break;
                            }

                            property = formatter.Read(ref reader);
                            property.Type = type;
                            break;
                    }
                }

                if (formatter == null)
                {
                    // skip unknown value
                    continue;
                }
                if (property == null)
                {
                    throw new InvalidOperationException("not found value");
                }
                property.Key = key ?? throw new InvalidOperationException("not found key");
                result.Add(property);
            }
            return result;
        }

        private IEnumerable<IProperty> readPropertiesValueWithShortType(ref JsonReader reader)
        {
            var result = new List<IProperty>();
            reader.ReadIsBeginObjectWithVerify();
            int count = 0;
            while (reader.ReadIsEndObjectWithSkipValueSeparator(ref count) == false)
            {
                string key = reader.ReadPropertyName();

                reader.ReadIsBeginObjectWithVerify();
                string type = reader.ReadPropertyName();

                IUtf8JsonFormatter formatter = utf8JsonFormatterResolver.Resolve(type);
                if (formatter == null)
                {
                    // skip unknown value
                    PrimitiveObjectFormatter.Default.Deserialize(ref reader, utf8JsonFormatterResolver.JsonFormatterResolver);
                    reader.ReadIsEndObjectWithVerify();
                    continue;
                }

                IProperty property = formatter.Read(ref reader);
                reader.ReadIsEndObjectWithVerify();

                property.Type = type;
                property.Key = key;
                result.Add(property);
            }
            return result;
        }

        public byte[] Serialize(IEnumerable<IProperty> properties)
        {
            var writer = new JsonWriter(MemoryPool.GetBuffer());
            writer.WriteBeginObject();

            writeProperties(ref writer, properties);

            writer.WriteEndObject();
            return writer.ToUtf8ByteArray();
        }

        private void writeProperties(ref JsonWriter writer, IEnumerable<IProperty> properties)
        {
            writer.WritePropertyName(SerializeConstant.VersionName);
            writer.WriteString(SerializeConstant.Version.ToString());
            writer.WriteValueSeparator();

            writer.WritePropertyName(SerializeConstant.PropertyTypeName);
            writer.WriteString(SerializeMode.ToPropertyTypeValue());
            writer.WriteValueSeparator();

            writer.WritePropertyName(SerializeConstant.PropertiesName);
            switch (SerializeMode)
            {
                case SerializeMode.LargeObject:
                    writePropertiesValueWithLargeType(ref writer, properties);
                    break;
                case SerializeMode.ShortObject:
                default:
                    writePropertiesValueWithShortType(ref writer, properties);
                    break;
            }
        }

        private void writePropertiesValueWithLargeType(ref JsonWriter writer, IEnumerable<IProperty> properties)
        {
            writer.WriteBeginArray();
            int count = 0;
            foreach (var property in properties)
            {
                if (0 < count)
                {
                    writer.WriteValueSeparator();
                }

                var utf8JsonFormatter = property.Formatter as IUtf8JsonFormatter ?? utf8JsonFormatterResolver.Resolve(property.Type);
                if (utf8JsonFormatter == null)
                {
                    continue;
                }

                writer.WriteBeginObject();

                writer.WritePropertyName(SerializeConstant.KeyName);
                writer.WriteString(property.Key);
                writer.WriteValueSeparator();

                writer.WritePropertyName(SerializeConstant.TypeName);
                writer.WriteString(property.Type);
                writer.WriteValueSeparator();

                writer.WritePropertyName(SerializeConstant.ValueName);
                utf8JsonFormatter.Write(ref writer, property);

                writer.WriteEndObject();
                count++;
            }
            writer.WriteEndArray();
        }

        private void writePropertiesValueWithShortType(ref JsonWriter writer, IEnumerable<IProperty> properties)
        {
            writer.WriteBeginObject();
            int count = 0;
            foreach (var property in properties)
            {
                if (0 < count)
                {
                    writer.WriteValueSeparator();
                }
                var utf8JsonFormatter = property.Formatter as IUtf8JsonFormatter ?? utf8JsonFormatterResolver.Resolve(property.Type);
                if (utf8JsonFormatter == null)
                {
                    continue;
                }

                writer.WritePropertyName(property.Key);

                writer.WriteBeginObject();
                writer.WritePropertyName(property.Type);
                utf8JsonFormatter.Write(ref writer, property);
                writer.WriteEndObject();

                count++;
            }
            writer.WriteEndObject();
        }
    }
}
