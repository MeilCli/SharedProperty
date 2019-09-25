# SharedProperty
[![Build Status](https://dev.azure.com/meilcli/SharedProperty/_apis/build/status/MeilCli.SharedProperty?branchName=master)](https://dev.azure.com/meilcli/SharedProperty/_build/latest?definitionId=3&branchName=master) [![nuget](https://img.shields.io/nuget/v/SharedProperty.svg)](https://www.nuget.org/packages/SharedProperty)  
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

get the value use `GetPropety` method or `TryGetProperty` method.
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
.NET Core SDK=3.0.100
  [Host] : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT
  Core   : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|                         Method |       Mean |     Error |     StdDev |     Median |        Min |        Max |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------- |-----------:|----------:|-----------:|-----------:|-----------:|-----------:|-------:|------:|------:|----------:|
|         ShortUtf8JsonSerialize |   920.3 ns | 23.433 ns |  68.726 ns |   883.2 ns |   842.8 ns | 1,030.2 ns | 0.2041 |     - |     - |     856 B |
|         LargeUtf8JsonSerialize | 1,178.5 ns | 55.216 ns |  65.731 ns | 1,147.1 ns | 1,118.8 ns | 1,308.9 ns | 0.0916 |     - |     - |     384 B |
|         ShortSpanJsonSerialize |   771.4 ns | 21.177 ns |  62.441 ns |   733.0 ns |   706.6 ns |   967.8 ns | 0.0763 |     - |     - |     320 B |
|         LargeSpanJsonSerialize | 1,383.7 ns | 34.000 ns | 100.250 ns | 1,432.0 ns | 1,258.5 ns | 1,675.9 ns | 0.0916 |     - |     - |     384 B |
|   ShortSystemTextJsonSerialize | 1,924.4 ns | 48.249 ns | 142.263 ns | 2,008.5 ns | 1,749.4 ns | 2,325.2 ns | 0.1469 |     - |     - |     616 B |
|   LargeSystemTextJsonSerialize | 2,232.5 ns | 21.685 ns |  19.223 ns | 2,229.3 ns | 2,204.5 ns | 2,269.5 ns | 0.1602 |     - |     - |     680 B |
|       ShortUtf8JsonDeserialize | 1,774.1 ns | 52.125 ns | 152.874 ns | 1,694.3 ns | 1,612.9 ns | 2,187.1 ns | 0.2174 |     - |     - |     912 B |
|       LargeUtf8JsonDeserialize | 2,353.2 ns | 50.123 ns | 106.817 ns | 2,315.9 ns | 2,246.3 ns | 2,666.1 ns | 0.2861 |     - |     - |    1200 B |
|       ShortSpanJsonDeserialize | 1,205.5 ns |  7.795 ns |   6.910 ns | 1,204.8 ns | 1,193.6 ns | 1,216.3 ns | 0.2174 |     - |     - |     912 B |
|       LargeSpanJsonDeserialize | 1,807.0 ns | 17.712 ns |  15.702 ns | 1,804.8 ns | 1,789.5 ns | 1,842.7 ns | 0.2861 |     - |     - |    1200 B |
| ShortSystemTextJsonDeserialize | 2,551.4 ns | 25.394 ns |  23.753 ns | 2,544.5 ns | 2,520.5 ns | 2,591.1 ns | 0.2213 |     - |     - |     928 B |
| LargeSystemTextJsonDeserialize | 3,376.1 ns | 29.346 ns |  27.450 ns | 3,371.6 ns | 3,335.8 ns | 3,432.8 ns | 0.2899 |     - |     - |    1216 B |

[SharedProperty.Benchmark.NETCore/SerializerBench.cs](SharedProperty.Benchmark.NETCore/SerializerBench.cs)

In addition, when using two Serializers mixed, it is necessary to use only the common function of Utf8Json and SpanJson.

## Storage
By default, `FileStorage` and `IsolatedFileStorage` are prepared.
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
Use `AesCryptoConverter.Default` or `RijndaelCryptoConverter.Default` for slight rich obfuscation.


### Encryption
Can use `AesCryptoConverter` or `RijndaelCryptoConverter` for encryption.
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
- [System.Buffers](https://www.nuget.org/packages/System.Buffers/) : published by [MIT License](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)

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
