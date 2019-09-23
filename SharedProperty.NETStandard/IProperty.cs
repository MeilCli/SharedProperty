using System.Diagnostics.CodeAnalysis;

namespace SharedProperty.NETStandard
{
    public interface IProperty
    {
        string? Key { get; set; }
        string? Type { get; set; }
        IFormatter? Formatter { get; }
        object? Value { get; }
    }

    public interface IProperty<T> : IProperty
    {
        [MaybeNull]
        [AllowNull]
        new T Value { get; set; }
    }
}
