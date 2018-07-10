namespace SharedProperty.NETStandard
{
    public class Property
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public IFormatter Formatter { get; set; }
    }

    public class Property<T> : Property
    {
        public T Value { get; set; }
    }
}
