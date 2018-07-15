using SharedProperty.NETStandard;
using System.Collections.Generic;
using System.Linq;

namespace SharedProperty.Test.NETCore
{
    public class EmptySerializer : ISerializer
    {
        public static readonly EmptySerializer Default = new EmptySerializer();

        public IFormatterResolver FormatterResolver => EmptyFormatterResolver.Default;

        public IEnumerable<IProperty> Deserialize(byte[] binary)
        {
            return Enumerable.Empty<IProperty>();
        }

        public byte[] Serialize(IEnumerable<IProperty> properties)
        {
            return new byte[0];
        }
    }
}
