namespace SharedProperty.NETStandard
{
    public interface IFormatterResolver
    {
        IFormatter Resolve<T>();
    }
}
