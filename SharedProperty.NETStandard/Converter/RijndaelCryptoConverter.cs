using System.Security.Cryptography;

namespace SharedProperty.NETStandard.Converter
{
    public class RijndaelCryptoConverter : SymmetricCryptoConverter
    {
        private static readonly RijndaelManaged defaultRijndaelManaged = new RijndaelManaged
        {
            KeySize = 256,
            BlockSize = 128,
            Key = Default256BitKey,
            IV = Default128BitIv,
            Mode = CipherMode.CBC,
            Padding = PaddingMode.PKCS7
        };
        public static readonly RijndaelCryptoConverter Default = new RijndaelCryptoConverter();

        public RijndaelCryptoConverter() : base(defaultRijndaelManaged) { }

        public RijndaelCryptoConverter(RijndaelManaged rijndaelManaged) : base(rijndaelManaged) { }
    }
}
