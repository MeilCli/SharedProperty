namespace SharedProperty.NETStandard
{
    public sealed class SharedDictionary : BaseSharedDictionary
    {
        public SharedDictionary(ISerializer serializer, IStorage? storage, IConverter? converter)
            : base(serializer, storage, converter)
        {
        }
    }
}