using SharedProperty.NETStandard;
using System;
using System.Collections.Generic;
using Utf8Json;

namespace SharedProperty.Serializer.Utf8Json
{
    public class Utf8JsonFormatterResolver : IFormatterResolver
    {
        internal readonly IJsonFormatterResolver JsonFormatterResolver;
        private readonly Dictionary<string, IUtf8JsonFormatter> formatterCache = new Dictionary<string, IUtf8JsonFormatter>();

        public Utf8JsonFormatterResolver() : this(global::Utf8Json.Resolvers.StandardResolver.Default) { }

        public Utf8JsonFormatterResolver(IJsonFormatterResolver jsonFormatterResolver)
        {
            this.JsonFormatterResolver = jsonFormatterResolver;
        }

        internal IUtf8JsonFormatter Resolve<T>()
        {
            if (formatterCache.TryGetValue(TypeCache<T>.FullName, out IUtf8JsonFormatter formatter))
            {
                return formatter;
            }
            else
            {
                formatter = new Utf8JsonFormatter<T>(JsonFormatterResolver);
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
                Type targetType = Type.GetType(fullNameType);
                if (targetType == null)
                {
                    return null;
                }

                Type formatterType = typeof(Utf8JsonFormatter<>).MakeGenericType(targetType);
                formatter = Activator.CreateInstance(formatterType, JsonFormatterResolver) as IUtf8JsonFormatter;
                formatterCache[fullNameType] = formatter;
                return formatter;
            }
        }
    }
}
