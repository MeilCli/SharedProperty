﻿using System;

namespace SharedProperty.NETStandard.TypeConverters
{
    public class UnsignedIntTypeConverter : ITypeConverter<uint>
    {
        /// <exception cref="System.InvalidOperationException">not support convert</exception>
        public uint ConvertAndGetValue(IProperty property)
        {
            if (property is IProperty<byte> byteProperty)
            {
                return byteProperty.Value;
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
