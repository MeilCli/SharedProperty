using SharedProperty.NETStandard;
using SpanJson;
using SpanJson.Resolvers;
using System;
using System.Collections.Generic;

namespace SharedProperty.Serializer.SpanJson
{
    public static class SpanJsonSerializer
    {
        public static readonly SpanJsonSerializer<ExcludeNullsOriginalCaseResolver<byte>> Default
            = new SpanJsonSerializer<ExcludeNullsOriginalCaseResolver<byte>>();

        public static SpanJsonSerializer<ExcludeNullsOriginalCaseResolver<byte>> Create(SerializeMode mode)
        {
            return new SpanJsonSerializer<ExcludeNullsOriginalCaseResolver<byte>>(mode);
        }
    }

    public class SpanJsonSerializer<TResolver> : ISerializer
        where TResolver : IJsonFormatterResolver<byte, TResolver>, new()
    {
        private readonly SpanJsonFormatterResolver<TResolver> jsonFormatterResolver;

        public IDictionary<string, string> MigrationTypeDictionary { get; } = new Dictionary<string, string>();

        public IFormatterResolver FormatterResolver {
            get => jsonFormatterResolver;
        }

        public SerializeMode SerializeMode { get; }

        public SpanJsonSerializer()
            : this(new SpanJsonFormatterResolver<TResolver>()) { }

        public SpanJsonSerializer(SpanJsonFormatterResolver<TResolver> jsonFormatterResolver)
            : this(jsonFormatterResolver, SerializeMode.ShortObject) { }

        public SpanJsonSerializer(SerializeMode serializeMode)
            : this(new SpanJsonFormatterResolver<TResolver>(), serializeMode) { }

        public SpanJsonSerializer(SpanJsonFormatterResolver<TResolver> jsonFormatterResolver, SerializeMode serializeMode)
        {
            this.jsonFormatterResolver = jsonFormatterResolver;
            SerializeMode = serializeMode;
        }

        public IEnumerable<IProperty> Deserialize(byte[] binary)
        {
            var reader = new JsonReader<byte>(binary);
            reader.ReadUtf8BeginObjectOrThrow();
            int count = 0;
            string propertyType = null;
            IEnumerable<IProperty> result = null;
            while (reader.TryReadUtf8IsEndObjectOrValueSeparator(ref count) == false)
            {
                string name = reader.ReadUtf8EscapedName();
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

        private IEnumerable<IProperty> readProperiesValue(ref JsonReader<byte> reader, string propertyType)
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

        private IEnumerable<IProperty> readPropertiesValueWithLargeType(ref JsonReader<byte> reader)
        {
            var result = new List<IProperty>();
            reader.ReadUtf8BeginArrayOrThrow();
            int arrayCount = 0;
            while (reader.TryReadUtf8IsEndArrayOrValueSeparator(ref arrayCount) == false)
            {
                string key = null;
                string type = null;
                IProperty property = null;
                ISpanJsonFormatter formatter = null;
                reader.ReadUtf8BeginObjectOrThrow();
                int objectCount = 0;
                while (reader.TryReadUtf8IsEndObjectOrValueSeparator(ref objectCount) == false)
                {
                    string name = reader.ReadUtf8EscapedName();
                    switch (name)
                    {
                        case SerializeConstant.KeyName:
                            key = reader.ReadString();
                            break;
                        case SerializeConstant.TypeName:
                            type = reader.ReadString();
                            if (MigrationTypeDictionary.TryGetValue(type, out string migrationType))
                            {
                                type = migrationType;
                            }
                            break;
                        case SerializeConstant.ValueName:
                            if (type == null)
                            {
                                throw new InvalidOperationException("value must be last json order");
                            }

                            formatter = jsonFormatterResolver.Resolve(type);
                            if (formatter == null)
                            {
                                reader.ReadUtf8Dynamic();
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

        private IEnumerable<IProperty> readPropertiesValueWithShortType(ref JsonReader<byte> reader)
        {
            var result = new List<IProperty>();
            reader.ReadUtf8BeginObjectOrThrow();
            int count = 0;
            while (reader.TryReadUtf8IsEndObjectOrValueSeparator(ref count) == false)
            {
                string key = reader.ReadUtf8EscapedName();

                reader.ReadUtf8BeginObjectOrThrow();
                string type = reader.ReadUtf8EscapedName();
                if (MigrationTypeDictionary.TryGetValue(type, out string migrationType))
                {
                    type = migrationType;
                }

                ISpanJsonFormatter formatter = jsonFormatterResolver.Resolve(type);
                if (formatter == null)
                {
                    // skip unknown value
                    reader.ReadUtf8Dynamic();
                    reader.ReadUtf8IsEndObject();
                    continue;
                }

                IProperty property = formatter.Read(ref reader);
                reader.ReadUtf8IsEndObject();

                property.Type = type;
                property.Key = key;
                result.Add(property);
            }
            return result;
        }

        public byte[] Serialize(IEnumerable<IProperty> properties)
        {
            var writer = new JsonWriter<byte>(short.MaxValue);
            writer.WriteBeginObject();

            writeProperties(ref writer, properties);

            writer.WriteEndObject();
            return writer.ToByteArray();
        }

        private void writeProperties(ref JsonWriter<byte> writer, IEnumerable<IProperty> properties)
        {
            writer.WriteUtf8Name(SerializeConstant.VersionName);
            writer.WriteUtf8String(SerializeConstant.Version.ToString());
            writer.WriteUtf8ValueSeparator();

            writer.WriteUtf8Name(SerializeConstant.PropertyTypeName);
            writer.WriteUtf8String(SerializeMode.ToPropertyTypeValue());
            writer.WriteUtf8ValueSeparator();

            writer.WriteUtf8Name(SerializeConstant.PropertiesName);
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

        private void writePropertiesValueWithLargeType(ref JsonWriter<byte> writer, IEnumerable<IProperty> properties)
        {
            writer.WriteUtf8BeginArray();
            int count = 0;
            foreach (var property in properties)
            {
                if (0 < count)
                {
                    writer.WriteUtf8ValueSeparator();
                }

                var spanJsonFormatter = property.Formatter as ISpanJsonFormatter ?? jsonFormatterResolver.Resolve(property.Type);
                if (spanJsonFormatter == null)
                {
                    continue;
                }

                writer.WriteUtf8BeginObject();

                writer.WriteUtf8Name(SerializeConstant.KeyName);
                writer.WriteUtf8String(property.Key);
                writer.WriteUtf8ValueSeparator();

                writer.WriteUtf8Name(SerializeConstant.TypeName);
                writer.WriteUtf8String(property.Type);
                writer.WriteUtf8ValueSeparator();

                writer.WriteUtf8Name(SerializeConstant.ValueName);
                spanJsonFormatter.Write(ref writer, property);

                writer.WriteUtf8EndObject();
                count++;
            }
            writer.WriteUtf8EndArray();
        }

        private void writePropertiesValueWithShortType(ref JsonWriter<byte> writer, IEnumerable<IProperty> properties)
        {
            writer.WriteUtf8BeginObject();
            int count = 0;
            foreach (var property in properties)
            {
                if (0 < count)
                {
                    writer.WriteUtf8ValueSeparator();
                }

                var spanJsonFormatter = property.Formatter as ISpanJsonFormatter ?? jsonFormatterResolver.Resolve(property.Type);
                if (spanJsonFormatter == null)
                {
                    continue;
                }

                writer.WriteUtf8Name(property.Key);

                writer.WriteUtf8BeginObject();
                writer.WriteUtf8Name(property.Type);
                spanJsonFormatter.Write(ref writer, property);
                writer.WriteUtf8EndObject();

                count++;
            }
            writer.WriteUtf8EndObject();
        }
    }
}
