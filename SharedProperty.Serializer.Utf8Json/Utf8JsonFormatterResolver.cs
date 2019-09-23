using System;
using System.Collections.Generic;
using SharedProperty.NETStandard;
using Utf8Json;

namespace SharedProperty.Serializer.Utf8Json
{
    public class Utf8JsonFormatterResolver : IFormatterResolver
    {
        /// <summary>
        /// creating StandardResolver must be method.
        /// otherwise, Xamarin.iOS application crush.
        /// </summary>
        /// <returns></returns>
        private static IJsonFormatterResolver createStandardResolver()
        {
            return global::Utf8Json.Resolvers.StandardResolver.Default;
        }

        internal readonly IJsonFormatterResolver JsonFormatterResolver;
        private readonly Dictionary<string, IUtf8JsonFormatter> formatterCache = new Dictionary<string, IUtf8JsonFormatter>();

        public Utf8JsonFormatterResolver(IJsonFormatterResolver? jsonFormatterResolver = null)
        {
            JsonFormatterResolver = jsonFormatterResolver ?? createStandardResolver();
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

        internal IUtf8JsonFormatter? Resolve(string? fullNameType)
        {
            if (fullNameType is null)
            {
                return null;
            }
            if (formatterCache.TryGetValue(fullNameType, out IUtf8JsonFormatter formatter))
            {
                return formatter;
            }
            else
            {
                Type targetType = Type.GetType(fullNameType);
                if (targetType is null)
                {
                    return null;
                }

                Type formatterType = typeof(Utf8JsonFormatter<>).MakeGenericType(targetType);
                var targetFormatter = Activator.CreateInstance(formatterType, JsonFormatterResolver) as IUtf8JsonFormatter;
                if (targetFormatter is null)
                {
                    return null;
                }
                formatterCache[fullNameType] = targetFormatter;
                return targetFormatter;
            }
        }
    }
}
