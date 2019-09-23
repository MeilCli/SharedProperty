# SharedProperty
[![Build Status](https://meilcli.visualstudio.com/SharedProperty/_apis/build/status/MeilCli.SharedProperty)](https://meilcli.visualstudio.com/SharedProperty/_build/latest?definitionId=3) [![nuget](https://img.shields.io/nuget/v/SharedProperty.svg)](https://www.nuget.org/packages/SharedProperty)  
Key-Value store library for application setting(for .NET Standard 2.0)

[for Japanese/日本語版README](README-ja.md)

## Operating environment
It should work if it is compatible with .NET Standard 2.0

- .NET Framework
- .NET Core
- Xamarin.Android
- Xamarin.iOS

## Install

- [SharedProperty](https://www.nuget.org/packages/SharedProperty)
- [SharedProperty.Serializer.SpanJson](https://www.nuget.org/packages/SharedProperty.Serializer.SpanJson)
  - must install if using SpanJson in Serializer
- [SharedProperty.Serializer.Utf8Json](https://www.nuget.org/packages/SharedProperty.Serializer.Utf8Json)
  - must install if using Utf8Json in Serializer
- [SharedProperty.Serializer.SystemTextJson](https://www.nuget.org/packages/SharedProperty.Serializer.SystemTextJson)
  - must install if using System.Text.Json in Serializer

## Usage
```csharp
var sharedDictionary = new SharedDictionary(Utf8JsonSerializer.Default, FileStorage.Default, null);
```
SharedDictionary constructor has 3 arguments.
- ISerializer
  - Required to Serialize, Deserialize to Json format
- IStorage
  - Required to save to storage
- IConverter
  - Used to obfuscate or encrypt when saving to storage

must load from storage if using saved storage value.
```csharp
await sharedDictionary.LoadFromStorageAsync();
```

and, must call save method when save to storage.
```csharp
await sharedDictionary.SaveToStorageAsync();
```

set the value use `SetProperty` method.
```csharp
sharedDictionary.SetProperty("text", "sssss");
sharedDictionary.SetProperty("number", 1234);
sharedDictionary.SetProperty("data", new Data());
sharedDictionary.SetProperty("list", new List<int> { 1, 2, 3, 4 });
```
restricted by the Json library used by each serializer if using collection or user defined type.

get the value use `GetPropety` method or ` TryGetProperty` method.
```csharp
WriteLine(sharedDictionary.GetProperty<string>("text"));
WriteLine(sharedDictionary.GetProperty<int>("number"));
WriteLine(sharedDictionary.GetProperty<Data>("data"));
WriteLine(sharedDictionary.GetProperty<List<int>>("list").Count);
```

## Serializer
SharedProperty provides the following two Serializers.

- SharedProperty.Serializer.Utf8Json
- SharedProperty.Serializer.SpanJson
- SharedProperty.Serializer.SystemTextJson

### Utf8Json
can use fast JsonSerializer : [Utf8Json](https://github.com/neuecc/Utf8Json) for .NET Standard.

Setting to SharedDictionary, use `Utf8JsonSerializer.Default` or create `Utf8JsonSerializer` instance.

Utf8Json solves each value when Serialize/Deserialize.

### SpanJson
can use fast JsonSerializer : [SpanJson](https://github.com/Tornhoof/SpanJson) for .NET Core 2.1.   
supporting UTF-8 mode because performance.

Setting to SharedDicitionary, use `SpanJsonSerializer.Default`  or create `SpanJsonSerializer` instance.

SpanJson solves each value when Serialize/Deserialize.

### System.Text.Json
can use fast JsonSerializer : [System.Text.Json](https://www.nuget.org/packages/System.Text.Json) for .NET Standard.   

Setting to SharedDicitionary, use `SystemTextJsonSerializer.Default`  or create `SystemTextJsonSerializer` instance.

System.Text.Json solves each value when Serialize/Deserialize.

### SerializeMode
SharedProperty provides two Json formats.

- SerializeMode.ShortObject
  - It eliminates unnecessary characters and reduces the size. It may be difficult for humans to edit.
- SerializeMode.LargeObject
  - It is a format adapted to ordinary Json data. If there is a possibility of human editing, this format may be good.

Default value is SerializeMode.ShortObject.

### How to choose Serializer and Mode
Applications for .NET Core 2.1 may wonder which Serializer to use.
The following benchmarks are helpful.
``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18362
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-rc1-014190
  [Host] : .NET Core 2.1.12 (CoreCLR 4.6.27817.01, CoreFX 4.6.27818.01), 64bit RyuJIT
  Core   : .NET Core 2.1.12 (CoreCLR 4.6.27817.01, CoreFX 4.6.27818.01), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|                         Method |       Mean |     Error |     StdDev |        Min |        Max |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------- |-----------:|----------:|-----------:|-----------:|-----------:|-------:|------:|------:|----------:|
|         ShortUtf8JsonSerialize |   825.4 ns |  8.604 ns |   7.627 ns |   814.0 ns |   841.2 ns | 0.0753 |     - |     - |     320 B |
|         LargeUtf8JsonSerialize | 1,196.8 ns | 10.690 ns |   9.476 ns | 1,185.4 ns | 1,219.2 ns | 0.0896 |     - |     - |     384 B |
|         ShortSpanJsonSerialize |   858.7 ns |  4.100 ns |   3.634 ns |   852.8 ns |   864.3 ns | 0.0753 |     - |     - |     320 B |
|         LargeSpanJsonSerialize | 1,481.1 ns |  7.335 ns |   6.125 ns | 1,468.4 ns | 1,491.7 ns | 0.0896 |     - |     - |     384 B |
|   ShortSystemTextJsonSerialize | 2,252.9 ns | 30.226 ns |  26.795 ns | 2,214.8 ns | 2,285.2 ns | 0.2937 |     - |     - |    1248 B |
|   LargeSystemTextJsonSerialize | 2,905.0 ns | 19.105 ns |  17.871 ns | 2,870.7 ns | 2,934.1 ns | 0.4387 |     - |     - |    1848 B |
|       ShortUtf8JsonDeserialize | 1,893.4 ns | 14.710 ns |  12.284 ns | 1,873.8 ns | 1,911.2 ns | 0.2270 |     - |     - |     960 B |
|       LargeUtf8JsonDeserialize | 2,664.7 ns | 22.682 ns |  21.217 ns | 2,634.5 ns | 2,716.4 ns | 0.3052 |     - |     - |    1296 B |
|       ShortSpanJsonDeserialize | 1,276.1 ns | 15.472 ns |  12.920 ns | 1,258.8 ns | 1,295.4 ns | 0.2270 |     - |     - |     960 B |
|       LargeSpanJsonDeserialize | 1,794.7 ns | 13.744 ns |  12.856 ns | 1,779.7 ns | 1,824.6 ns | 0.3071 |     - |     - |    1296 B |
| ShortSystemTextJsonDeserialize | 3,060.5 ns | 59.927 ns |  73.595 ns | 2,973.1 ns | 3,271.5 ns | 0.2289 |     - |     - |     976 B |
| LargeSystemTextJsonDeserialize | 4,492.6 ns | 96.066 ns | 134.671 ns | 4,298.8 ns | 4,900.0 ns | 0.3090 |     - |     - |    1312 B |

[SharedProperty.Benchmark.NETCore/SerializerBench.cs](SharedProperty.Benchmark.NETCore/SerializerBench.cs)

In addition, when using two Serializers mixed, it is necessary to use only the common function of Utf8Json and SpanJson.

## Storage
By default, `FileStorage` and` IsolatedFileStorage` are prepared.
### FileStorage
Specify the storage location by file path. Use it when you want to save it in the same directory as the exe file.

### IsolatedFileStorage
Recommend using `IsolatedFileStorage` when saving application data.
In cross-platform execution environments such as Xamarin, .NET Standard will resolve the save directory.
Therefore, specify the storage location by filename.

## Converter
For obfuscation and encryption, standard Converter is available.

### Obfuscation
Converter for obfuscation prepares the following.

- `SimpleConverter.Default`
- `AesCryptoConverter.Default`
- `RijndaelCryptoConverter.Default`

`SimpleConverter.Default` is simply very fast because it simply increments the character code.
Use `AesCryptoConverter.Default` or` RijndaelCryptoConverter.Default` for slight rich obfuscation.


### Encryption
Can use `AesCryptoConverter` or` RijndaelCryptoConverter` for encryption.
In order to maintain strong security, developers must always use application-specific encryption keys.

## Caution
### Thread Safe
`SharedDictionary` class is not thread-safe. To operate with multiple threads, use the `ConcurrentSharedDictionary` class.


### Implicit type conversion
Currently SharedProperty allows implicit type conversion when retrieving values.

```csharp
int i = 1234;
sharedDictionary.SetProperty("number", i);
// Internally it is held as an int
long l = sharedDictionary.GetProperty<long>("number")
```

Can also write code like this.

However, performance is not good. Also, since it is also a function that may be deleted in the future, **recommend specifying the exact type.**

Supported implicit type conversion
- Up cast
- Covariance cast and Contravariance cast
- Nullable cast
- [Implicit numeric conversion](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/implicit-numeric-conversions-table)
- implicit operator


### Migration
It is possible to migrate the saved value type by using `Utf8JsonSerializer.MigrationTypeDictionary` or`SpanJsonSerializer.MigrationTypeDictionary` or `SystemTextJsonSerializer.MigrationTypeDictionary`.

Also, for migration with the same type please create Utf8Json or SpanJson CustomFormatterResolver and CustomFormatter.

Also, reading of unknown type data will be skipped when loading the file.

### Xamarin.iOS
In Xamarin.iOS the application crashes when using `Utf8Json.Resolvers.StandardResolver.Default` because AOT.

Have a `AotStandardResolver` class, but please use Utf8Json code generator for user defined types.

## License
This library is published by [MIT License](LICENSE).

#### Package
The library used depends on the package.

**SharedProperty.Serializer.Utf8Json**
- [Utf8Json](https://github.com/neuecc/Utf8Json) : published by [MIT License](https://github.com/neuecc/Utf8Json/blob/master/LICENSE).

**SharedProperty.Serializer.SpanJson**
- [SpanJson](https://github.com/Tornhoof/SpanJson) : published by [MIT License](https://github.com/Tornhoof/SpanJson/blob/master/LICENSE).


**SharedProperty.Serializer.SystemTextJson**
- [System.Text.Json](https://www.nuget.org/packages/System.Text.Json) : published by [MIT License](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT).

#### Benchmark
The following libraries are used for the benchmark.
- [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) : published by [MIT License](https://github.com/dotnet/BenchmarkDotNet/blob/master/LICENSE.md).

#### Sample
The following libraries are used for the sample.
- [Utf8Json](https://github.com/neuecc/Utf8Json) : published by [MIT License](https://github.com/neuecc/Utf8Json/blob/master/LICENSE).
- [Xamarin.Forms](https://github.com/xamarin/Xamarin.Forms) : published by [MIT License](https://github.com/xamarin/Xamarin.Forms/blob/master/LICENSE).
- [AndroidSupportComponents](https://github.com/xamarin/AndroidSupportComponents) : published by [MIT License](https://github.com/xamarin/AndroidSupportComponents/blob/master/LICENSE.md).
