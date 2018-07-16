using System;

namespace SharedProperty.NETStandard.TypeConverters
{
    public class UnsignedLongTypeConverter : ITypeConverter<ulong>
    {
        /// <exception cref="System.InvalidOperationException">not support convert</exception>
        public ulong ConvertAndGetValue(IProperty property)
        {
            if (property is IProperty<byte> byteProperty)
            {
                return byteProperty.Value;
            }

            if (property is IProperty<ushort> ushortProperty)
            {
                return ushortProperty.Value;
            }

            if (property is IProperty<uint> uintProperty)
            {
                return uintProperty.Value;
            }

            if (property is IProperty<char> charProperty)
            {
                return charProperty.Value;
            }

            throw new InvalidOperationException("not cast type");
        }
    }
}
