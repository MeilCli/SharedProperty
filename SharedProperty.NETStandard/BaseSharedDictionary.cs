using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SharedProperty.NETStandard
{
    public class BaseSharedDictionary : ISharedDictionary
    {
        protected readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly ISerializer serializer;
        private readonly IFormatterResolver formatterResolver;
        private readonly IStorage storage;
        private readonly IConverter converter;
        private readonly Dictionary<string, IProperty> properties = new Dictionary<string, IProperty>();

        public virtual int PropertyCount => properties.Count;

        public BaseSharedDictionary(ISerializer serializer, IStorage storage, IConverter converter)
        {
            this.serializer = serializer;
            formatterResolver = serializer.FormatterResolver;
            this.storage = storage;
            this.converter = converter;
        }

        public async Task LoadFromStorageAsync()
        {
            await SemaphoreSlim.WaitAsync().ConfigureAwait(false);
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
                SemaphoreSlim.Release();
            }
        }

        public async Task SaveToStorageAsync()
        {
            await SemaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                byte[] bytes = serializer.Serialize(properties.Values);
                bytes = converter?.Convert(bytes) ?? bytes;
                await storage.WriteAsync(bytes);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public virtual byte[] RawExport()
        {
            return serializer.Serialize(properties.Values);
        }

        public virtual void RawImport(byte[] binary)
        {
            properties.Clear();
            foreach (var property in serializer.Deserialize(binary))
            {
                properties[property.Key] = property;
            }
        }

        public virtual bool ContainsProperty(string key)
        {
            return properties.ContainsKey(key);
        }

        public virtual T GetProperty<T>(string key)
        {
            if (properties.TryGetValue(key, out IProperty property) == false)
            {
                throw new KeyNotFoundException($"not found key: {key}");
            }

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

            // implicit operating cast
            if (TypeCache<T>.CanImplicitOperatingConvert(property.Type))
            {
                return TypeCache<T>.GetPropertyConvertAndGetValueDelegate(property.Type)(property);
            }

            throw new InvalidOperationException($"not target typed value");
        }

        public virtual bool TryGetProperty<T>(string key, out T value)
        {
            if (properties.TryGetValue(key, out IProperty property) == false)
            {
                value = default;
                return false;
            }

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

            // implicit operating cast
            if (TypeCache<T>.CanImplicitOperatingConvert(property.Type))
            {
                value = TypeCache<T>.GetPropertyConvertAndGetValueDelegate(property.Type)(property);
                return true;
            }

            value = default;
            return false;
        }

        public virtual void SetProperty<T>(string key, T value)
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

        public virtual bool RemoveProperty(string key)
        {
            return properties.Remove(key);
        }

        public virtual void ClearProperty()
        {
            properties.Clear();
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
