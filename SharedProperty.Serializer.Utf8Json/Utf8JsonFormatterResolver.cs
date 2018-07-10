using SharedProperty.NETStandard;
using System;
using System.Collections.Generic;
using Utf8Json;

namespace SharedProperty.Serializer.Utf8Json
{
    public class Utf8JsonFormatterResolver : IFormatterResolver
    {
        private readonly IJsonFormatterResolver jsonFormatterResolver;
        private readonly Dictionary<string, IUtf8JsonFormatter> formatterCache = new Dictionary<string, IUtf8JsonFormatter>();

        public Utf8JsonFormatterResolver() : this(global::Utf8Json.Resolvers.StandardResolver.Default) { }

        public Utf8JsonFormatterResolver(IJsonFormatterResolver jsonFormatterResolver)
        {
            this.jsonFormatterResolver = jsonFormatterResolver;
        }

        internal IUtf8JsonFormatter Resolve<T>()
        {
            if (formatterCache.TryGetValue(TypeCache<T>.FullName, out IUtf8JsonFormatter formatter))
            {
                return formatter;
            }
            else
            {
                formatter = new Utf8JsonFormatter<T>(jsonFormatterResolver);
                formatterCache[TypeCache<T>.FullName] = formatter;
                return formatter;
            }
        }

        IFormatter IFormatterResolver.Resolve<T>()
        {
            return Resolve<T>();
        }

        internal IUtf8JsonFormatter Resolve(string fullNameType)
        {
            if (formatterCache.TryGetValue(fullNameType, out IUtf8JsonFormatter formatter))
            {
                return formatter;
            }
            else
            {
                Type formatterType = typeof(Utf8JsonFormatter<>).MakeGenericType(Type.GetType(fullNameType));
                formatter = Activator.CreateInstance(formatterType, jsonFormatterResolver) as IUtf8JsonFormatter;
                formatterCache[fullNameType] = formatter;
                return formatter;
            }
        }
    }
}
