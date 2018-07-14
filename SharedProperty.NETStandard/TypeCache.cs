using SharedProperty.NETStandard.Extensions;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace SharedProperty.NETStandard
{
    public static class TypeCache<T>
    {
        public static readonly string FullName;
        private static readonly ConcurrentDictionary<string, bool> canImplicitOperatingConvertTypes
            = new ConcurrentDictionary<string, bool>();
        private static readonly ConcurrentDictionary<string, Func<IProperty, T>> propertyConvertAndGetValueDelegates
            = new ConcurrentDictionary<string, Func<IProperty, T>>();

        static TypeCache()
        {
            FullName = toFullName(typeof(T));
        }

        public static bool CanImplicitOperatingConvert(string sourceType)
        {
            if (canImplicitOperatingConvertTypes.TryGetValue(sourceType, out bool result))
            {
                return result;
            }

            Type type = Type.GetType(sourceType);
            Type targetType = typeof(T);
            result = type.CanImplicitOperatingConvert(targetType);
            canImplicitOperatingConvertTypes[sourceType] = result;

            return result;
        }

        public static Func<IProperty, T> GetPropertyConvertAndGetValueDelegate(string sourceType)
        {
            if (propertyConvertAndGetValueDelegates.TryGetValue(sourceType, out Func<IProperty, T> result))
            {
                return result;
            }

            Type type = Type.GetType(sourceType);
            result = type.CreatePropertyConvertAndGetValueDelegate<T>();
            propertyConvertAndGetValueDelegates[sourceType] = result;

            return result;
        }

        private static string toFullName(Type type)
        {
            var sb = new StringBuilder();
            sb.Append(type.Namespace);
            sb.Append('.');
            sb.Append(type.Name);

            if (0 < type.GenericTypeArguments.Length)
            {
                sb.Append('[');
                int count = 0;
                foreach (var genericType in type.GenericTypeArguments)
                {
                    if (0 < count)
                    {
                        sb.Append(',');
                    }
                    sb.Append('[');
                    sb.Append(toFullName(genericType));
                    sb.Append(']');
                    count++;
                }
                sb.Append(']');
            }

            sb.Append(',');
            sb.Append(' ');
            string assemblyFullName = type.Assembly.FullName;
            sb.Append(assemblyFullName.Substring(0, assemblyFullName.IndexOf(',')));
            return sb.ToString();
        }
    }
}
