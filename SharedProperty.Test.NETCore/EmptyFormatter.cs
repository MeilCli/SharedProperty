using SharedProperty.NETStandard;

namespace SharedProperty.Test.NETCore
{
    public class EmptyFormatter : IFormatter
    {
        public static readonly EmptyFormatter Default = new EmptyFormatter();
    }
}
