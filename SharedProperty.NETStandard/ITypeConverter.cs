namespace SharedProperty.NETStandard
{
    public interface ITypeConverter
    {
    }

    public interface ITypeConverter<T> : ITypeConverter
    {
        /// <exception cref="System.InvalidOperationException">not support convert</exception>
        T ConvertAndGetValue(IProperty property);
    }
}
