using System;
using System.Text;

namespace SharedProperty.NETStandard
{
    public static class TypeCache<T>
    {
        public static readonly string FullName;

        static TypeCache()
        {
            FullName = toFullName(typeof(T));
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
