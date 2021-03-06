﻿using BenchmarkDotNet.Attributes;
using SharedProperty.NETStandard;
using SharedProperty.Serializer.SpanJson;
using SharedProperty.Serializer.SystemTextJson;
using SharedProperty.Serializer.Utf8Json;

namespace SharedProperty.Benchmark.NETCore
{
    [CoreJob]
    [MeanColumn, MinColumn, MaxColumn]
    [MemoryDiagnoser]
    public class SerializerBench
    {
        private readonly ISharedDictionary shortUtf8JsonSerializeSharedDictionary
            = new SharedDictionary(new Utf8JsonSerializer(SerializeMode.ShortObject), null, null);
        private readonly ISharedDictionary largeUtf8JsonSerializeSharedDictionary
            = new SharedDictionary(new Utf8JsonSerializer(SerializeMode.LargeObject), null, null);
        private readonly ISharedDictionary shortSpanJsonSerializeSharedDictionary
            = new SharedDictionary(SpanJsonSerializer.Create(SerializeMode.ShortObject), null, null);
        private readonly ISharedDictionary largeSpanJsonSerializeSharedDictionary
            = new SharedDictionary(SpanJsonSerializer.Create(SerializeMode.LargeObject), null, null);
        private readonly ISharedDictionary shortSystemTextJsonSerializeSharedDictionary
            = new SharedDictionary(new SystemTextJsonSerializer(SerializeMode.ShortObject), null, null);
        private readonly ISharedDictionary largeSystemTextJsonSerializeSharedDictionary
            = new SharedDictionary(new SystemTextJsonSerializer(SerializeMode.LargeObject), null, null);

#pragma warning disable CS8618
        private byte[] shortJsonBytes;
        private byte[] largeJsonBytes;
#pragma warning restore CS8618

        private readonly ISharedDictionary shortUtf8JsonDeserializeSharedDictionary
            = new SharedDictionary(new Utf8JsonSerializer(SerializeMode.ShortObject), null, null);
        private readonly ISharedDictionary largeUtf8JsonDeserializeSharedDictionary
            = new SharedDictionary(new Utf8JsonSerializer(SerializeMode.LargeObject), null, null);
        private readonly ISharedDictionary shortSpanJsonDeserializeSharedDictionary
            = new SharedDictionary(SpanJsonSerializer.Create(SerializeMode.ShortObject), null, null);
        private readonly ISharedDictionary largeSpanJsonDeserializeSharedDictionary
            = new SharedDictionary(SpanJsonSerializer.Create(SerializeMode.LargeObject), null, null);
        private readonly ISharedDictionary shortSystemTextJsonDeserializeSharedDictionary
            = new SharedDictionary(new SystemTextJsonSerializer(SerializeMode.ShortObject), null, null);
        private readonly ISharedDictionary largeSystemTextJsonDeserializeSharedDictionary
            = new SharedDictionary(new SystemTextJsonSerializer(SerializeMode.LargeObject), null, null);

        [GlobalSetup]
        public void SetUp()
        {
            var sharedDictionaries = new[] {
                shortUtf8JsonSerializeSharedDictionary,
                largeUtf8JsonSerializeSharedDictionary,
                shortSpanJsonSerializeSharedDictionary,
                largeSpanJsonSerializeSharedDictionary,
                shortSystemTextJsonSerializeSharedDictionary,
                largeSystemTextJsonSerializeSharedDictionary
            };

            foreach (var sharedDictionary in sharedDictionaries)
            {
                sharedDictionary.SetProperty("string", "string");
                sharedDictionary.SetProperty("bool", true);
                sharedDictionary.SetProperty("int", 1);
            }

            shortJsonBytes = shortUtf8JsonSerializeSharedDictionary.RawExport();
            largeJsonBytes = largeUtf8JsonSerializeSharedDictionary.RawExport();
        }

        [Benchmark]
        public void ShortUtf8JsonSerialize()
        {
            shortUtf8JsonSerializeSharedDictionary.RawExport();
        }

        [Benchmark]
        public void LargeUtf8JsonSerialize()
        {
            largeUtf8JsonSerializeSharedDictionary.RawExport();
        }

        [Benchmark]
        public void ShortSpanJsonSerialize()
        {
            shortSpanJsonSerializeSharedDictionary.RawExport();
        }

        [Benchmark]
        public void LargeSpanJsonSerialize()
        {
            largeSpanJsonSerializeSharedDictionary.RawExport();
        }

        [Benchmark]
        public void ShortSystemTextJsonSerialize()
        {
            shortSystemTextJsonSerializeSharedDictionary.RawExport();
        }

        [Benchmark]
        public void LargeSystemTextJsonSerialize()
        {
            largeSystemTextJsonSerializeSharedDictionary.RawExport();
        }

        [Benchmark]
        public void ShortUtf8JsonDeserialize()
        {
            shortUtf8JsonDeserializeSharedDictionary.RawImport(shortJsonBytes);
        }

        [Benchmark]
        public void LargeUtf8JsonDeserialize()
        {
            largeUtf8JsonDeserializeSharedDictionary.RawImport(largeJsonBytes);
        }

        [Benchmark]
        public void ShortSpanJsonDeserialize()
        {
            shortSpanJsonDeserializeSharedDictionary.RawImport(shortJsonBytes);
        }

        [Benchmark]
        public void LargeSpanJsonDeserialize()
        {
            largeSpanJsonDeserializeSharedDictionary.RawImport(largeJsonBytes);
        }

        [Benchmark]
        public void ShortSystemTextJsonDeserialize()
        {
            shortSystemTextJsonDeserializeSharedDictionary.RawImport(shortJsonBytes);
        }

        [Benchmark]
        public void LargeSystemTextJsonDeserialize()
        {
            largeSystemTextJsonDeserializeSharedDictionary.RawImport(largeJsonBytes);
        }
    }
}
