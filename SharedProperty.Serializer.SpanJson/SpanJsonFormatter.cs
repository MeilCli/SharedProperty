using System;
using SharedProperty.NETStandard;
using SpanJson;

namespace SharedProperty.Serializer.SpanJson
{
    class SpanJsonFormatter<T, TResolver> : ISpanJsonFormatter
        where TResolver : IJsonFormatterResolver<byte, TResolver>, new()
    {
        private readonly IJsonFormatterResolver<byte, TResolver> jsonFormatterResolver;
        private readonly IJsonFormatter<T, byte> jsonFormatter;

        public SpanJsonFormatter(IJsonFormatterResolver<byte, TResolver> jsonFormatterResolver)
        {
            this.jsonFormatterResolver = jsonFormatterResolver;
            jsonFormatter = jsonFormatterResolver.GetFormatter<T>();
        }

        public void Write(ref JsonWriter<byte> writer, IProperty property)
        {
            if (property is Property<T> typedProperty)
            {
                jsonFormatter.Serialize(ref writer, typedProperty.Value);
            }
            else
            {
                throw new InvalidOperationException($"property type parameter is not {typeof(T).FullName}");
            }
        }

        public IProperty Read(ref JsonReader<byte> reader)
        {
            T value = jsonFormatter.Deserialize(ref reader);
            return new Property<T>
            {
                Formatter = this,
                Value = value
            };
        }
    }
}
