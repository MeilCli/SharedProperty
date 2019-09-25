using SharedProperty.NETStandard;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SharedProperty.Serializer.SystemTextJson
{
    public class SystemTextJsonSerializer : ISerializer
    {
        private static class MemoryHelper
        {
            [ThreadStatic]
            private static int memorySize = byte.MaxValue;

            public static int MemorySize => memorySize;

            public static void SetLastUseMemorySize(int size)
            {
                memorySize = size;
            }
        }

        private static SystemTextJsonSerializer? _default;

        public static SystemTextJsonSerializer Default {
            get {
                _default ??= new SystemTextJsonSerializer();
                return _default;
            }
        }

        private readonly SystemTextJsonFormatterResolver systemTextJsonFormatterResolver;

        public IDictionary<string, string> MigrationTypeDictionary { get; } = new Dictionary<string, string>();

        public IFormatterResolver FormatterResolver {
            get => systemTextJsonFormatterResolver;
        }

        public SerializeMode SerializeMode { get; }

        public SystemTextJsonSerializer()
            : this(new SystemTextJsonFormatterResolver(new JsonSerializerOptions())) { }

        public SystemTextJsonSerializer(SystemTextJsonFormatterResolver jsonFormatterResolver)
            : this(jsonFormatterResolver, SerializeMode.ShortObject) { }

        public SystemTextJsonSerializer(SerializeMode serializeMode)
            : this(new SystemTextJsonFormatterResolver(new JsonSerializerOptions()), serializeMode) { }

        public SystemTextJsonSerializer(SystemTextJsonFormatterResolver jsonFormatterResolver, SerializeMode serializeMode)
        {
            this.systemTextJsonFormatterResolver = jsonFormatterResolver;
            SerializeMode = serializeMode;
        }

        public IEnumerable<IProperty> Deserialize(byte[] binary)
        {
            var readerOption = new JsonReaderOptions()
            {
                AllowTrailingCommas = systemTextJsonFormatterResolver.JsonSerializerOptions.AllowTrailingCommas,
                CommentHandling = systemTextJsonFormatterResolver.JsonSerializerOptions.ReadCommentHandling,
                MaxDepth = systemTextJsonFormatterResolver.JsonSerializerOptions.MaxDepth
            };
            var reader = new Utf8JsonReader(binary, readerOption);
            string? propertyType = null;
            IEnumerable<IProperty>? result = null;
            while (reader.Read())
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    continue;
                }
                string name = reader.GetString();

                if (reader.Read() is false)
                {
                    break;
                }
                switch (name)
                {
                    case SerializeConstant.VersionName:
                        break;
                    case SerializeConstant.PropertyTypeName:
                        propertyType = reader.GetString();
                        break;
                    case SerializeConstant.PropertiesName:
                        result = readProperiesValue(ref reader, propertyType);
                        break;
                }
            }
            return result ?? throw new InvalidOperationException("not found properties");
        }

        private IEnumerable<IProperty>? readProperiesValue(ref Utf8JsonReader reader, string? propertyType)
        {
            if (propertyType is null)
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

        private IEnumerable<IProperty> readPropertiesValueWithLargeType(ref Utf8JsonReader reader)
        {
            var result = new List<IProperty>();
            while (reader.Read())
            {
                string? key = null;
                string? type = null;
                IProperty? property = null;
                ISystemTextJsonFormatter? formatter = null;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        continue;
                    }

                    string name = reader.GetString();

                    if (reader.Read() is false)
                    {
                        break;
                    }
                    switch (name)
                    {
                        case SerializeConstant.KeyName:
                            key = reader.GetString();
                            break;
                        case SerializeConstant.TypeName:
                            type = reader.GetString();
                            if (MigrationTypeDictionary.TryGetValue(type, out string migrationType))
                            {
                                type = migrationType;
                            }
                            break;
                        case SerializeConstant.ValueName:
                            if (type is null)
                            {
                                throw new InvalidOperationException("value must be last json order");
                            }

                            formatter = systemTextJsonFormatterResolver.Resolve(type);
                            if (formatter is null)
                            {
                                break;
                            }

                            property = formatter.Read(ref reader);
                            property.Type = type;
                            break;
                    }
                }

                if (formatter is null)
                {
                    // skip unknown value
                    continue;
                }
                if (property is null)
                {
                    throw new InvalidOperationException("not found value");
                }
                property.Key = key ?? throw new InvalidOperationException("not found key");
                result.Add(property);
            }

            return result;
        }

        private IEnumerable<IProperty> readPropertiesValueWithShortType(ref Utf8JsonReader reader)
        {
            var result = new List<IProperty>();
            while (reader.Read())
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    continue;
                }
                string key = reader.GetString();

                reader.Read();
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new InvalidOperationException("invalid json object");
                }

                reader.Read();
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new InvalidOperationException("invalid json property");
                }
                string type = reader.GetString();

                ISystemTextJsonFormatter? formatter = systemTextJsonFormatterResolver.Resolve(type);
                if (formatter is null)
                {
                    // skip unknown value
                    continue;
                }

                reader.Read();
                IProperty property = formatter.Read(ref reader);
                property.Type = type;
                property.Key = key;
                result.Add(property);
            }
            return result;
        }

        public byte[] Serialize(IEnumerable<IProperty> properties)
        {
            var writerOption = new JsonWriterOptions()
            {
                Encoder = systemTextJsonFormatterResolver.JsonSerializerOptions.Encoder,
                Indented = systemTextJsonFormatterResolver.JsonSerializerOptions.WriteIndented,
                SkipValidation = true
            };
            var arrayBufferWriter = new ArrayBufferWriter<byte>(MemoryHelper.MemorySize);
            var writer = new Utf8JsonWriter(arrayBufferWriter, writerOption);

            writer.WriteStartObject();
            writeProperties(ref writer, properties);
            writer.WriteEndObject();
            writer.Flush();


            byte[] result = arrayBufferWriter.WrittenMemory.ToArray();
            MemoryHelper.SetLastUseMemorySize(result.Length);
            arrayBufferWriter.Dispose();
            return result;
        }

        private void writeProperties(ref Utf8JsonWriter writer, IEnumerable<IProperty> properties)
        {
            writer.WritePropertyName(SerializeConstant.VersionName);
            writer.WriteStringValue(SerializeConstant.Version.ToString());

            writer.WritePropertyName(SerializeConstant.PropertyTypeName);
            writer.WriteStringValue(SerializeMode.ToPropertyTypeValue());

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

        private void writePropertiesValueWithLargeType(ref Utf8JsonWriter writer, IEnumerable<IProperty> properties)
        {
            writer.WriteStartArray();
            foreach (var property in properties)
            {
                var systemTextJsonFormatter = property.Formatter as ISystemTextJsonFormatter ?? systemTextJsonFormatterResolver.Resolve(property.Type);
                if (systemTextJsonFormatter is null)
                {
                    continue;
                }

                writer.WriteStartObject();

                writer.WritePropertyName(SerializeConstant.KeyName);
                writer.WriteStringValue(property.Key);

                writer.WritePropertyName(SerializeConstant.TypeName);
                writer.WriteStringValue(property.Type);

                writer.WritePropertyName(SerializeConstant.ValueName);
                systemTextJsonFormatter.Write(ref writer, property);

                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        private void writePropertiesValueWithShortType(ref Utf8JsonWriter writer, IEnumerable<IProperty> properties)
        {
            writer.WriteStartObject();

            foreach (var property in properties)
            {
                var systemTextJsonFormatter = property.Formatter as ISystemTextJsonFormatter ?? systemTextJsonFormatterResolver.Resolve(property.Type);
                if (systemTextJsonFormatter is null)
                {
                    continue;
                }

                writer.WritePropertyName(property.Key);

                writer.WriteStartObject();
                writer.WritePropertyName(property.Type);
                systemTextJsonFormatter.Write(ref writer, property);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }
}
