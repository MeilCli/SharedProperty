using System;

namespace SharedProperty.NETStandard.TypeConverters
{
    public class UnsignedShortTypeConverter : ITypeConverter<ushort>
    {
        public ushort ConvertAndGetValue(IProperty property)
        {
            if (property is IProperty<byte> byteProperty)
            {
                return byteProperty.Value;
            }

            if (property is IProperty<char> charProperty)
            {
                return charProperty.Value;
            }

            throw new InvalidOperationException("not cast type");
        }
    }
}
