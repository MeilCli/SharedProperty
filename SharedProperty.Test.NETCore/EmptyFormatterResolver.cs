using SharedProperty.NETStandard;

namespace SharedProperty.Test.NETCore
{
    public class EmptyFormatterResolver : IFormatterResolver
    {
        public static readonly EmptyFormatterResolver Default = new EmptyFormatterResolver();

        public IFormatter Resolve<T>()
        {
            return EmptyFormatter.Default;
        }
    }
}
