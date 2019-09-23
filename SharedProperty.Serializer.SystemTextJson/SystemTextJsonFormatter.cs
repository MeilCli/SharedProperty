using System;
using System.Text.Json;
using SharedProperty.NETStandard;

namespace SharedProperty.Serializer.SystemTextJson
{
    class SystemTextJsonFormatter<T> : ISystemTextJsonFormatter
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public SystemTextJsonFormatter(JsonSerializerOptions jsonSerializerOptions)
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public void Write(ref Utf8JsonWriter writer, IProperty property)
        {
            if (property is Property<T> typedProperty)
            {
                JsonSerializer.Serialize(writer, typedProperty.Value, jsonSerializerOptions);
            }
            else
            {
                throw new InvalidOperationException($"property type parameter is not {typeof(T).FullName}");
            }
        }

        public IProperty Read(ref Utf8JsonReader reader)
        {
            T value = JsonSerializer.Deserialize<T>(ref reader, jsonSerializerOptions);
            return new Property<T>(this)
            {
                Value = value
            };
        }
    }
}
