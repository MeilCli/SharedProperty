using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedProperty.NETStandard
{
    public interface ISharedDictionary : IEnumerable<KeyValuePair<string, object>>
    {
        int PropertyCount { get; }

        Task LoadFromStorageAsync();

        Task SaveToStorageAsync();

        byte[] RawExport();

        void RawImport(byte[] binary);

        bool ContainsProperty(string key);

        /// <exception cref="System.Collections.Generic.KeyNotFoundException">not found key</exception>
        /// <exception cref="System.InvalidOperationException">not target typed value or not support convert</exception>
        T GetProperty<T>(string key);

        bool TryGetProperty<T>(string key, out T value);

        void SetProperty<T>(string key, T value);

        bool RemoveProperty(string key);

        void ClearProperty();
    }
}
