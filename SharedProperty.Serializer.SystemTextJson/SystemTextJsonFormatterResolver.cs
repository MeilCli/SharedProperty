using System;
using System.Collections.Generic;
using System.Text.Json;
using SharedProperty.NETStandard;

namespace SharedProperty.Serializer.SystemTextJson
{
    public class SystemTextJsonFormatterResolver : IFormatterResolver
    {
        internal readonly JsonSerializerOptions JsonSerializerOptions;
        private readonly Dictionary<string, ISystemTextJsonFormatter> formatterCache = new Dictionary<string, ISystemTextJsonFormatter>();

        public SystemTextJsonFormatterResolver(JsonSerializerOptions jsonSerializerOptions)
        {
            this.JsonSerializerOptions = jsonSerializerOptions;
        }

        internal ISystemTextJsonFormatter Resolve<T>()
        {
            if (formatterCache.TryGetValue(TypeCache<T>.FullName, out ISystemTextJsonFormatter formatter))
            {
                return formatter;
            }
            else
            {
                formatter = new SystemTextJsonFormatter<T>(JsonSerializerOptions);
                formatterCache[TypeCache<T>.FullName] = formatter;
                return formatter;
            }
        }

        IFormatter IFormatterResolver.Resolve<T>()
        {
            return Resolve<T>();
        }

        internal ISystemTextJsonFormatter? Resolve(string? fullNameType)
        {
            if (fullNameType is null)
            {
                return null;
            }
            if (formatterCache.TryGetValue(fullNameType, out ISystemTextJsonFormatter formatter))
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

                Type formatterType = typeof(SystemTextJsonFormatter<>).MakeGenericType(targetType);
                var targetFormatter = Activator.CreateInstance(formatterType, JsonSerializerOptions) as ISystemTextJsonFormatter;
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
