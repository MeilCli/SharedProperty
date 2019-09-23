using Utf8Json;
using Utf8Json.Resolvers;

namespace SharedProperty.Serializer.Utf8Json
{
    public class AotStandardResolver : IJsonFormatterResolver
    {
        private static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T>? Formatter;

            static FormatterCache()
            {
                foreach (var resolver in InnerResolvers)
                {
                    Formatter = resolver.GetFormatter<T>();
                    if (Formatter != null)
                    {
                        break;
                    }
                }
            }
        }

        private static readonly IJsonFormatterResolver[] InnerResolvers = new IJsonFormatterResolver[]
        {
            BuiltinResolver.Instance,
            EnumResolver.Default,
            DynamicGenericResolver.Instance,
            AttributeFormatterResolver.Instance
        };

        public static readonly AotStandardResolver Default = new AotStandardResolver();

        public IJsonFormatter<T>? GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }
    }
}
