namespace SharedProperty.NETStandard
{
    public interface ITypeConverter
    {
    }

    public interface ITypeConverter<T> : ITypeConverter
    {
        T ConvertAndGetValue(IProperty property);
    }
}
