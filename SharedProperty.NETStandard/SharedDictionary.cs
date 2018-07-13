using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharedProperty.NETStandard
{
    public class SharedDictionary : ISharedDictionary
    {
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly ISerializer serializer;
        private readonly IFormatterResolver formatterResolver;
        private readonly IStorage storage;
        private readonly IConverter converter;
        private readonly Dictionary<string, IProperty> properties = new Dictionary<string, IProperty>();

        public SharedDictionary(ISerializer serializer, IStorage storage, IConverter converter)
        {
            this.serializer = serializer;
            formatterResolver = serializer.FormatterResolver;
            this.storage = storage;
            this.converter = converter;
        }

        public async Task LoadFromStorageAsync()
        {
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                properties.Clear();

                if (storage.Exists() == false)
                {
                    return;
                }

                byte[] bytes = await storage.ReadAsync();
                bytes = converter?.Deconvert(bytes) ?? bytes;
                foreach (var property in serializer.Deserialize(bytes))
                {
                    properties[property.Key] = property;
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task SaveToStorageAsync()
        {
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                byte[] bytes = serializer.Serialize(properties.Values);
                bytes = converter?.Convert(bytes) ?? bytes;
                await storage.WriteAsync(bytes);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public byte[] RawExport()
        {
            return serializer.Serialize(properties.Values);
        }

        public void RawImport(byte[] binary)
        {
            properties.Clear();
            foreach (var property in serializer.Deserialize(binary))
            {
                properties[property.Key] = property;
            }
        }

        public bool ContainsKey(string key)
        {
            return properties.ContainsKey(key);
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                return ContainsKey(key);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public T GetProperty<T>(string key)
        {
            properties.TryGetValue(key, out IProperty property);
            if (property is IProperty<T> typedProperty)
            {
                return typedProperty.Value;
            }

            // Upcast and Covariance cast and Contravariance cast and Nullable cast
            if (property.Value is T)
            {
                return (T)property.Value;
            }

            // implicit number cast
            ITypeConverter typeConverter = TypeConverterCache<T>.TypeConverter;
            if (typeConverter is ITypeConverter<T> typedTypeConverter)
            {
                return typedTypeConverter.ConvertAndGetValue(property);
            }

            // ToDo: implicit cast check(Using reflection Op_implicit method)

            throw new KeyNotFoundException($"not found key:{key} or value");
        }

        public async Task<T> GetPropertyAsync<T>(string key)
        {
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                return GetProperty<T>(key);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public bool TryGetProperty<T>(string key, out T value)
        {
            properties.TryGetValue(key, out IProperty property);
            if (property is IProperty<T> typedProperty)
            {
                value = typedProperty.Value;
                return true;
            }

            // Upcast and Covariance cast and Contravariance cast and Nullable cast
            if (property.Value is T)
            {
                value = (T)property.Value;
                return true;
            }

            // implicit number cast
            ITypeConverter typeConverter = TypeConverterCache<T>.TypeConverter;
            if (typeConverter is ITypeConverter<T> typedTypeConverter)
            {
                try
                {
                    value = typedTypeConverter.ConvertAndGetValue(property);
                    return true;
                }
                catch (Exception)
                {
                    value = default;
                    return false;
                }
            }

            // ToDo: implicit cast check(Using reflection Op_implicit method)

            value = default;
            return false;
        }

        public async Task<(bool isSuccess, T value)> TryGetPropertyAsync<T>(string key)
        {
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                bool isSuccess = TryGetProperty<T>(key, out T value);
                return (isSuccess, value);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public void SetProperty<T>(string key, T value)
        {
            if (properties.TryGetValue(key, out IProperty v) && v is IProperty<T> targetProperty)
            {
                targetProperty.Value = value;
                return;
            }

            var property = new Property<T>
            {
                Key = key,
                Type = TypeCache<T>.FullName,
                Formatter = formatterResolver.Resolve<T>(),
                Value = value
            };
            properties[key] = property;
        }

        public async Task SetPropertyAsync<T>(string key, T value)
        {
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                SetProperty(key, value);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public IEnumerator<IProperty> GetEnumerator()
        {
            return properties.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}