namespace SharedProperty.NETStandard
{
    public class Property<T> : IProperty<T>
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public IFormatter Formatter { get; set; }

        object IProperty.Value => Value;
        public T Value { get; set; }
    }
}
