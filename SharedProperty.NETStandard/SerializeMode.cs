namespace SharedProperty.NETStandard
{
    public enum SerializeMode
    {
        LargeObject, ShortObject
    }

    public static class SerializeModeExtension
    {
        public static string ToPropertyTypeValue(this SerializeMode mode)
        {
            switch (mode)
            {
                case SerializeMode.LargeObject:
                    return SerializeConstant.PropertyTypeLargeValue;
                case SerializeMode.ShortObject:
                default:
                    return SerializeConstant.PropertyTypeShortValue;
            }
        }
    }
}
