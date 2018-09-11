# SharedProperty
[![Build Status](https://meilcli.visualstudio.com/SharedProperty/_apis/build/status/MeilCli.SharedProperty)](https://meilcli.visualstudio.com/SharedProperty/_build/latest?definitionId=3) [![nuget](https://img.shields.io/nuget/v/SharedProperty.svg)](https://www.nuget.org/packages/SharedProperty) [![release](https://img.shields.io/github/release/MeilCli/SharedProperty/all.svg)](https://github.com/MeilCli/SharedProperty/releases)  
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

### Utf8Json
can use fast JsonSerializer : [Utf8Json](https://github.com/neuecc/Utf8Json) for .NET Standard.

Setting to SharedDictionary, use `Utf8JsonSerializer.Default` or create `Utf8JsonSerializer` instance.

Utf8Json solves each value when Serialize/Deserialize.

### SpanJson
can use fast JsonSerializer : [SpanJson](https://github.com/Tornhoof/SpanJson) for .NET Core 2.1.   
supporting UTF-8 mode because performance.

Setting to SharedDicitionary, use `SpanJsonSerializer.Default`  or create `SpanJsonSerializer` instance.

SpanJson solves each value when Serialize/Deserialize.

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

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=3328120 Hz, Resolution=300.4699 ns, Timer=TSC
.NET Core SDK=2.1.300
  [Host] : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Core   : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|                   Method |       Mean |     Error |    StdDev |        Min |        Max |  Gen 0 | Allocated |
|------------------------- |-----------:|----------:|----------:|-----------:|-----------:|-------:|----------:|
|   ShortUtf8JsonSerialize |   812.0 ns |  8.212 ns |  7.682 ns |   801.4 ns |   829.5 ns | 0.0753 |     320 B |
|   LargeUtf8JsonSerialize | 1,171.0 ns | 15.844 ns | 14.820 ns | 1,152.7 ns | 1,200.2 ns | 0.0896 |     384 B |
|   ShortSpanJsonSerialize |   839.1 ns |  5.006 ns |  4.438 ns |   828.6 ns |   845.4 ns | 0.0753 |     320 B |
|   LargeSpanJsonSerialize | 1,444.2 ns | 16.301 ns | 15.248 ns | 1,422.5 ns | 1,478.6 ns | 0.0896 |     384 B |
| ShortUtf8JsonDeserialize | 1,932.6 ns | 35.826 ns | 33.512 ns | 1,892.8 ns | 2,013.5 ns | 0.2251 |     960 B |
| LargeUtf8JsonDeserialize | 2,628.0 ns | 52.275 ns | 81.386 ns | 2,534.7 ns | 2,835.7 ns | 0.3052 |    1296 B |
| ShortSpanJsonDeserialize | 1,237.6 ns | 11.074 ns | 10.359 ns | 1,220.5 ns | 1,259.4 ns | 0.2270 |     960 B |
| LargeSpanJsonDeserialize | 1,710.6 ns |  8.564 ns |  8.011 ns | 1,701.5 ns | 1,723.7 ns | 0.3071 |    1296 B |

[SharedProperty.Benchmark.NETCore/SerializerBench.cs](SharedProperty.Benchmark.NETCore/SerializerBench.cs)

In addition, when using two Serializers mixed, it is necessary to use only the common function of Utf 8 Json and SpanJson.

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
It is possible to migrate the saved value type by using `Utf8JsonSerializer.MigrationTypeDictionary` or`SpanJsonSerializer.MigrationTypeDictionary`.

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

#### Benchmark
The following libraries are used for the benchmark.
- [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) : published by [MIT License](https://github.com/dotnet/BenchmarkDotNet/blob/master/LICENSE.md).

#### Sample
The following libraries are used for the sample.
- [Utf8Json](https://github.com/neuecc/Utf8Json) : published by [MIT License](https://github.com/neuecc/Utf8Json/blob/master/LICENSE).
- [Xamarin.Forms](https://github.com/xamarin/Xamarin.Forms) : published by [MIT License](https://github.com/xamarin/Xamarin.Forms/blob/master/LICENSE).
- [AndroidSupportComponents](https://github.com/xamarin/AndroidSupportComponents) : published by [MIT License](https://github.com/xamarin/AndroidSupportComponents/blob/master/LICENSE.md).
