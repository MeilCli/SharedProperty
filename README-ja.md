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
- [System.Buffers](https://www.nuget.org/packages/System.Buffers/) : [MIT License](https://github.com/dotnet/corefx/blob/master/LICENSE.TXT)によって公開されています

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
