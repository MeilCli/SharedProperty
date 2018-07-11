namespace SharedProperty.NETStandard
{
    public interface IProperty
    {
        string Key { get; set; }
        string Type { get; set; }
        IFormatter Formatter { get; set; }
        object Value { get; }
    }

    public interface IProperty<T> : IProperty
    {
        new T Value { get; set; }
    }
}
