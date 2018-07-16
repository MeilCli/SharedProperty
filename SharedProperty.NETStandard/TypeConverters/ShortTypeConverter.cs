using System;

namespace SharedProperty.NETStandard.TypeConverters
{
    public class ShortTypeConverter : ITypeConverter<short>
    {
        /// <exception cref="System.InvalidOperationException">not support convert</exception>
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
