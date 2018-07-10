namespace SharedProperty.NETStandard
{
    public interface IConverter
    {
        byte[] Convert(byte[] bytes);

        byte[] Deconvert(byte[] bytes);
    }
}
