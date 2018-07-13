using System.Security.Cryptography;

namespace SharedProperty.NETStandard.Converters
{
    public class AesCryptoConverter : SymmetricCryptoConverter
    {
        private static readonly AesManaged defaultAesManaged = new AesManaged
        {
            KeySize = 256,
            BlockSize = 128,
            Key = Default256BitKey,
            IV = Default128BitIv,
            Mode = CipherMode.CBC,
            Padding = PaddingMode.PKCS7
        };
        public static readonly AesCryptoConverter Default = new AesCryptoConverter();

        public AesCryptoConverter() : base(defaultAesManaged) { }

        public AesCryptoConverter(AesManaged aesManaged) : base(aesManaged) { }
    }
}
