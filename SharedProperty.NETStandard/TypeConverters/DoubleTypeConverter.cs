using System;

namespace SharedProperty.NETStandard.TypeConverters
{
    public class DoubleTypeConverter : ITypeConverter<double>
    {
        public double ConvertAndGetValue(IProperty property)
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

            if (property is IProperty<int> intProperty)
            {
                return intProperty.Value;
            }
            if (property is IProperty<uint> uintProperty)
            {
                return uintProperty.Value;
            }

            if (property is IProperty<long> longProperty)
            {
                return longProperty.Value;
            }
            if (property is IProperty<ulong> ulongProperty)
            {
                return ulongProperty.Value;
            }

            if (property is IProperty<char> charProperty)
            {
                return charProperty.Value;
            }

            if (property is IProperty<float> floatProperty)
            {
                return floatProperty.Value;
            }

            throw new InvalidOperationException("not cast type");
        }
    }
}
