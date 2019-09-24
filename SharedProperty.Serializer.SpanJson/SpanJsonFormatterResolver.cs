using SharedProperty.NETStandard;
using SpanJson;
using SpanJson.Resolvers;
using System;
using System.Collections.Generic;

namespace SharedProperty.Serializer.SpanJson
{
    public class SpanJsonFormatterResolver<TResolver> : IFormatterResolver
        where TResolver : IJsonFormatterResolver<byte, TResolver>, new()
    {
        private readonly IJsonFormatterResolver<byte, TResolver> jsonFormatterResolver;
        private readonly Dictionary<string, ISpanJsonFormatter> formatterCache = new Dictionary<string, ISpanJsonFormatter>();

        public SpanJsonFormatterResolver() : this(StandardResolvers.GetResolver<byte, TResolver>()) { }

        public SpanJsonFormatterResolver(IJsonFormatterResolver<byte, TResolver> jsonFormatterResolver)
        {
            this.jsonFormatterResolver = jsonFormatterResolver;
        }

        internal ISpanJsonFormatter Resolve<T>()
        {
            if (formatterCache.TryGetValue(TypeCache<T>.FullName, out ISpanJsonFormatter? formatter))
            {
                return formatter;
            }
            else
            {
                formatter = new SpanJsonFormatter<T, TResolver>(jsonFormatterResolver);
                formatterCache[TypeCache<T>.FullName] = formatter;
                return formatter;
            }
        }

        IFormatter IFormatterResolver.Resolve<T>()
        {
            return Resolve<T>();
        }

        internal ISpanJsonFormatter? Resolve(string? fullNameType)
        {
            if (fullNameType is null)
            {
                return null;
            }
            if (formatterCache.TryGetValue(fullNameType, out ISpanJsonFormatter? formatter))
            {
                return formatter;
            }
            else
            {
                Type? targetType = Type.GetType(fullNameType);
                if (targetType is null)
                {
                    return null;
                }

                Type formatterType = typeof(SpanJsonFormatter<,>).MakeGenericType(targetType, typeof(TResolver));
                var targetFormatter = Activator.CreateInstance(formatterType, jsonFormatterResolver) as ISpanJsonFormatter;
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
