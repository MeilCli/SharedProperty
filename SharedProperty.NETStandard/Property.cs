using System.Diagnostics.CodeAnalysis;

namespace SharedProperty.NETStandard
{
    public class Property<T> : IProperty<T>
    {
        public string? Key { get; set; }
        public string? Type { get; set; }
        public IFormatter Formatter { get; }

        object? IProperty.Value => Value;

        [AllowNull]
        [MaybeNull]
#pragma warning disable CS8618
        public T Value { get; set; }

        public Property(IFormatter formatter)
        {
            Formatter = formatter;
        }
#pragma warning restore CS8618
    }
}
