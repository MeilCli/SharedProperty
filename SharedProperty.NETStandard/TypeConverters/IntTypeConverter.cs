using System;

namespace SharedProperty.NETStandard.TypeConverters
{
    public class IntTypeConverter : ITypeConverter<int>
    {
        /// <exception cref="System.InvalidOperationException">not support convert</exception>
        public int ConvertAndGetValue(IProperty property)
        {
            if (property is IProperty<byte> byteProperty)
            {
                return byteProperty.Value;
            }
            if (property is IProperty<sbyte> sbyteProperty)
            {
                return sbyteProperty.Value;
            }

            if (property is IProperty<short> shortProperty)
            {
                return shortProperty.Value;
            }
            if (property is IProperty<ushort> ushortProperty)
            {
                return ushortProperty.Value;
            }

            if (property is IProperty<char> charProperty)
            {
                return charProperty.Value;
            }

            throw new InvalidOperationException("not cast type");
        }
    }
}
