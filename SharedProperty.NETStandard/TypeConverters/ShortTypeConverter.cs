using System;

namespace SharedProperty.NETStandard.TypeConverters
{
    public class ShortTypeConverter : ITypeConverter<short>
    {
        public short ConvertAndGetValue(IProperty property)
        {
            if (property is IProperty<byte> byteProperty)
            {
                return byteProperty.Value;
            }
            if (property is IProperty<sbyte> sbyteProperty)
            {
                return sbyteProperty.Value;
            }

            throw new InvalidOperationException("not cast type");
        }
    }
}
