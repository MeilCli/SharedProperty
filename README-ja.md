# SharedProperty
[![Build Status](https://dev.azure.com/meilcli/SharedProperty/_apis/build/status/MeilCli.SharedProperty?branchName=master)](https://dev.azure.com/meilcli/SharedProperty/_build/latest?definitionId=3&branchName=master) [![nuget](https://img.shields.io/nuget/v/SharedProperty.svg)](https://www.nuget.org/packages/SharedProperty)  
アプリケーションデータを保存するためのKey-Valueストアライブラリー(for .NET Standard 2.0)

## 動作環境
.NET Standard 2.0に対応している環境なら動作するはずです。

- .NET Framework
- .NET Core
- Xamarin.Android
- Xamarin.iOS

## インストール

- [SharedProperty](https://www.nuget.org/packages/SharedProperty)
- [SharedProperty.Serializer.SpanJson](https://www.nuget.org/packages/SharedProperty.Serializer.SpanJson)
  - SerializerにSpanJsonを使う場合に必要
- [SharedProperty.Serializer.Utf8Json](https://www.nuget.org/packages/SharedProperty.Serializer.Utf8Json)
  - SerializerにUtf8Jsonを使う場合に必要
- [SharedProperty.Serializer.SystemTextJson](https://www.nuget.org/packages/SharedProperty.Serializer.SystemTextJson)
  - SerializerにSystem.Text.Jsonを使う場合に必要

## 使い方
```csharp
var sharedDictionary = new SharedDictionary(Utf8JsonSerializer.Default, FileStorage.Default, null);
```
SharedDictionaryは三つの引数があります。
- ISerializer
  - Json形式にSerialize, Deserializeするのに必要
- IStorage
  - ストレージへ保存するのに必要
- IConverter
  - ストレージ保存時、難読化や暗号化するために利用

ストレージに保存されている値を利用するには必ずストレージから読み取る必要があります。
```csharp
await sharedDictionary.LoadFromStorageAsync();
```

また、ストレージに保存する際は必ずストレージに書き込む必要があります。
```csharp
await sharedDictionary.SaveToStorageAsync();
```

値を設定するには`SetProperty`メソッドを利用します。
```csharp
sharedDictionary.SetProperty("text", "sssss");
sharedDictionary.SetProperty("number", 1234);
sharedDictionary.SetProperty("data", new Data());
sharedDictionary.SetProperty("list", new List<int> { 1, 2, 3, 4 });
```
コレクションやユーザー定義型なども利用できますが、各Serializerが利用しているJsonライブラリーによって制限を受けます。

値を取得するには`GetPropety`メソッドまたは`TryGetProperty`メソッドを利用します。
```csharp
WriteLine(sharedDictionary.GetProperty<string>("text"));
WriteLine(sharedDictionary.GetProperty<int>("number"));
WriteLine(sharedDictionary.GetProperty<Data>("data"));
WriteLine(sharedDictionary.GetProperty<List<int>>("list").Count);
```

## Serializer
現在、SharedPropertyは以下の二つのSerializerを用意しています。

- SharedProperty.Serializer.Utf8Json
- SharedProperty.Serializer.SpanJson
- SharedProperty.Serializer.SystemTextJson

### Utf8Json
.NET Standard向けの高速なJsonSerializerである[Utf8Json](https://github.com/neuecc/Utf8Json)を使用することができます。

SharedDictionaryへの設定には、`Utf8JsonSerializer.Default`を使用するか`Utf8JsonSerializer`のインスタンスを作成してください。

各設定値のSerialize/DeserializeはUtf8Jsonが解決します。

### SpanJson
.NET Core 2.1向けの高速なJsonSerializerである[SpanJson](https://github.com/Tornhoof/SpanJson)を使用することができます。 
SpanJsonではUTF-16にも対応していますが速度が遅くなるため、UTF-8のみのサポートとしています。

SharedDicitionaryへの設定には、`SpanJsonSerializer.Default`を使用するか、`SpanJsonSerializer.Create`メソッドを使用するなどして`SpanJsonSerializer`のインスタンスを作成してください。  

各設定値のSerialize/DeserializeはSpanJsonが解決します。

### System.Text.Json
.NET Standard向けの高速なJsonSerializerである[System.Text.Json](https://www.nuget.org/packages/System.Text.Json)を使用することができます。

SharedDictionaryへの設定には、`SystemTextJsonSerializer.Default`を使用するか`SystemTextJsonSerializer`のインスタンスを作成してください。

各設定値のSerialize/DeserializeはSystem.Text.Jsonが解決します。

### SerializeMode
SharedPropertyは二つのJson形式を用意しています。

- SerializeMode.ShortObject
  - 不要な文字を排除し、サイズを減らした形式です。人間が編集するには難しいかもしれません。
- SerializeMode.LargeObject
  - 通常のJsonデータに合わせた形式です。人間が編集する可能性があるならば、こちらの形式がいいかもしれません。

デフォルト値ではSerializeMode.ShortObjectが設定されています。

### SerializerとModeの選び方
.NET Core 2.1向けアプリケーションでは、どちらのSerializerを利用するか悩むかもしれません。
以下のベンチマークが参考になります。
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

また、二つのSerializerを混合して使用する場合は、Utf8JsonとSpanJsonの共通の機能のみを使用する必要があります。

## Storage
標準では`FileStorage`と`IsolatedFileStorage`を用意しています。 
### FileStorage
ファイルパスによって保存場所を指定します。exeファイルと同じディレクトリに保存したい場合などに利用してください。

### IsolatedFileStorage
アプリケーションデータを保存する場合は`IsolatedFileStorage`の使用を勧めます。
Xamarinなどのクロスプラットフォームな実行環境において.NET Standardが保存ディレクトリを解決します。
そのため、ファイルの名前によって保存場所を指定します。

## Converter
難読化や暗号化のために標準ではいくつかのConverterを用意しています。

### 難読化
難読化のためのConverterは以下のものを用意しています。

- `SimpleConverter.Default`
- `AesCryptoConverter.Default`
- `RijndaelCryptoConverter.Default`

`SimpleConverter.Default`は単純に文字コードを+1しているだけなので非常に高速です。ちょっとリッチな難読化をする場合は`AesCryptoConverter.Default`または`RijndaelCryptoConverter.Default`を利用してください。


### 暗号化
暗号化には`AesCryptoConverter`または`RijndaelCryptoConverter`が使用できます。  
強度なセキュリティを保つには開発者は必ずアプリケーション固有の暗号鍵を使用する必要があります。

## 注意
### スレッドセーフ
`SharedDictionary`クラスはスレッドセーフではありません。複数スレッドで操作する場合は、`ConcurrentSharedDictionary`クラスを使用してください。


### 暗黙的型変換
現在、SharedPropertyでは値の取得時に暗黙的型変換が可能です。

```csharp
int i = 1234;
sharedDictionary.SetProperty("number", i);
// 内部的にはintで保持されている
long l = sharedDictionary.GetProperty<long>("number")
```

このような、コードを書くこともできます。

しかし、パフォーマンスがいいとは言えません。また、今後削除する可能性のある機能でもあるので、**正確に型指定することを推奨します。**

対応している暗黙的型変換
- アップキャスト
- 共変性・反変性
- Nullable化
- [暗黙的な数値変換](https://docs.microsoft.com/ja-jp/dotnet/csharp/language-reference/keywords/implicit-numeric-conversions-table)
- implicit operator


### マイグレーション
`Utf8JsonSerializer.MigrationTypeDictionary`または`SpanJsonSerializer.MigrationTypeDictionary`または`SystemTextJsonSerializer.MigrationTypeDictionary`を使用することによって保存した値の型をマイグレーションすることが可能です。
また、同じ型でのマイグレーションはUtf8JsonまたはSpanJsonのCustomFormatterResolverとCustomFormatterを作成してください。

また、ファイル読み込み時に不明な型のデータは読み込みがスキップされます。

### Xamarin.iOS
Xamarin.iOSではAOTのため`Utf8Json.Resolvers.StandardResolver.Default`を利用するとアプリケーションがクラッシュします。

`AotStandardResolver`クラスを用意していますが、ユーザー定義型はUtf8Jsonのコードジェネレーターを利用してください。

## ライセンス
このライブラリーは[MIT License](LICENSE)によって公開されています。

#### パッケージ
パッケージによって使用するライブラリが異なります。

**SharedProperty.Serializer.Utf8Json**
- [Utf8Json](https://github.com/neuecc/Utf8Json) : [MIT License](https://github.com/neuecc/Utf8Json/blob/master/LICENSE)によって公開されています

**SharedProperty.Serializer.SpanJson**
- [SpanJson](https://github.com/Tornhoof/SpanJson) : [MIT License](https://github.com/Tornhoof/SpanJson/blob/master/LICENSE)によって公開されています

**SharedProperty.Serializer.SystemTextJson**
- [System.Text.Json](https://www.nuget.org/packages/System.Text.Json) : [MIT License](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)によって公開されています

#### ベンチマーク
ベンチマークには以下のライブラリを使用しています。
- [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) : [MIT License](https://github.com/dotnet/BenchmarkDotNet/blob/master/LICENSE.md)によって公開されています。

#### サンプル
サンプルには以下のライブラリを使用しています。
- [Utf8Json](https://github.com/neuecc/Utf8Json) : [MIT License](https://github.com/neuecc/Utf8Json/blob/master/LICENSE)によって公開されています
- [Xamarin.Forms](https://github.com/xamarin/Xamarin.Forms) : [MIT License](https://github.com/xamarin/Xamarin.Forms/blob/master/LICENSE)によって公開されています。
- [AndroidSupportComponents](https://github.com/xamarin/AndroidSupportComponents) : [MIT License](https://github.com/xamarin/AndroidSupportComponents/blob/master/LICENSE.md)によって公開されています。
