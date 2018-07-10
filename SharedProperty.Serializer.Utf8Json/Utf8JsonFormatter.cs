﻿using SharedProperty.NETStandard;
using System;
using Utf8Json;

namespace SharedProperty.Serializer.Utf8Json
{
    class Utf8JsonFormatter<T> : IUtf8JsonFormatter
    {
        private readonly IJsonFormatterResolver jsonFormatterResolver;
        private readonly IJsonFormatter<T> jsonFormatter;

        public Utf8JsonFormatter(IJsonFormatterResolver jsonFormatterResolver)
        {
            this.jsonFormatterResolver = jsonFormatterResolver;
            jsonFormatter = jsonFormatterResolver.GetFormatterWithVerify<T>();
        }

        public void Write(ref JsonWriter writer, Property property)
        {
            if (property is Property<T> typedProperty)
            {
                jsonFormatter.Serialize(ref writer, typedProperty.Value, jsonFormatterResolver);
            }
            else
            {
                throw new InvalidOperationException($"property type parameter is not {typeof(T).FullName}");
            }
        }

        public Property Read(ref JsonReader reader)
        {
            T value = jsonFormatter.Deserialize(ref reader, jsonFormatterResolver);
            return new Property<T>
            {
                Formatter = this,
                Value = value
            };
        }
    }
}
