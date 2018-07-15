# SharedProperty
アプリケーションデータを保存するための.NET Standard 2.0ライブラリー

製作途中なのでAPIとかは変わる可能性があります、またAPIリクエストも受け付けています。

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

### Utf8Json
.NET Standard向けの高速なJsonSerializerである[Utf8Json](https://github.com/neuecc/Utf8Json)を使用することができます。

SharedDictionaryへの設定には、`Utf8JsonSerializer.Default`を使用するか`Utf8JsonSerializer`のインスタンスを作成してください。

各設定値のSerialize/DeserializeはUtf8Jsonが解決します。

### SpanJson
.NET Core 2.1向けの高速なJsonSerializerである[SpanJson](https://github.com/Tornhoof/SpanJson)を使用することができます。 
SpanJsonではUTF-16にも対応していますが速度が遅くなるため、UTF-8のみのサポートとしています。

SharedDicitionaryへの設定には、`SpanJsonSerializer.Default`を使用するか、`SpanJsonSerializer.Create`メソッドを使用するなどして`SpanJsonSerializer`のインスタンスを作成してください。  

各設定値のSerialize/DeserializeはSpanJsonが解決します。

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
|   ShortUtf8JsonSerialize |   752.7 ns |  9.349 ns |  8.287 ns |   740.3 ns |   767.3 ns | 0.0753 |     320 B |
|   LargeUtf8JsonSerialize | 1,121.4 ns |  6.662 ns |  5.905 ns | 1,111.8 ns | 1,130.4 ns | 0.0896 |     384 B |
|   ShortSpanJsonSerialize |   778.9 ns |  6.469 ns |  6.051 ns |   772.3 ns |   793.6 ns | 0.0753 |     320 B |
|   LargeSpanJsonSerialize | 1,364.4 ns | 17.396 ns | 15.421 ns | 1,348.1 ns | 1,404.4 ns | 0.0896 |     384 B |
| ShortUtf8JsonDeserialize | 1,743.9 ns | 12.912 ns | 12.078 ns | 1,721.0 ns | 1,765.8 ns | 0.2270 |     960 B |
| LargeUtf8JsonDeserialize | 2,415.6 ns | 19.449 ns | 17.241 ns | 2,392.9 ns | 2,442.9 ns | 0.3052 |    1296 B |
| ShortSpanJsonDeserialize | 1,184.5 ns | 19.126 ns | 15.971 ns | 1,161.9 ns | 1,218.4 ns | 0.2270 |     960 B |
| LargeSpanJsonDeserialize | 1,634.8 ns | 16.720 ns | 12.090 ns | 1,608.7 ns | 1,648.9 ns | 0.3071 |    1296 B |

[SharedProperty.Benchmark.NETCore/SerializerBench.cs](SharedProperty.Benchmark.NETCore/SerializerBench.cs)

また、二つのSerializerを混合して使用する場合は、Utf8JsonとSpanJsonの共通の機能のみを使用する必要があります。

## Storage
標準では`FileStorage`と`IsolatedFileStorage`を用意しています。  
アプリケーションデータ固有のデータは`IsolatedFileStorage`の利用を勧めますが、ファイルパスで保存場所を指定したい場合は`FileStorage`を利用してください。

## Converter
難読化や暗号化のために標準ではいくつかのConverterを用意しています。

難読化のためなら`SimpleConverter.Default`, `AesCryptoConverter.Default`, `RijndaelCryptoConverter.Default`が使えます。
`SimpleConverter.Default`は単純に文字コードを+1しているだけなので非常に高速です。ちょっとリッチな難読化をする場合は`AesCryptoConverter.Default`または`RijndaelCryptoConverter.Default`を利用してください。

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
SharedPropertyはデータのマイグレーションに対応していません。  
そのため、ユーザー定義型を保存する際は注意する必要があります。

トークンやIdなどの情報はなるべく基本データ型で保存し、ユーザー定義型は別ファイルを設定したSharedDictionaryに保存するのがより安全です。

また、ファイル読み込み時に不明な型のデータは読み込みがスキップされます。

### Xamarin.iOS
Xamarin.iOSではAOTのため`Utf8Json.Resolvers.StandardResolver.Default`を利用するとアプリケーションがクラッシュします。

`AotStandardResolver`クラスを用意していますが、ユーザー定義型はUtf8Jsonのコードジェネレーターを利用してください。

## ライセンス
このライブラリーは[MIT License](LICENSE.txt)によって公開されています。

#### パッケージ
パッケージによって使用するライブラリが異なります。

**SharedProperty.Serializer.Utf8Json**
- [Utf8Json](https://github.com/neuecc/Utf8Json) : [MIT License](https://github.com/neuecc/Utf8Json/blob/master/LICENSE)によって公開されています

**SharedProperty.Serializer.SpanJson**
- [SpanJson](https://github.com/Tornhoof/SpanJson) : [MIT License](https://github.com/Tornhoof/SpanJson/blob/master/LICENSE)によって公開されています

#### ベンチマーク
ベンチマークには以下のライブラリを使用しています。
- [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) : [MIT License](https://github.com/dotnet/BenchmarkDotNet/blob/master/LICENSE.md)によって公開されています。

#### サンプル
サンプルには以下のライブラリを使用しています。
- [Utf8Json](https://github.com/neuecc/Utf8Json) : [MIT License](https://github.com/neuecc/Utf8Json/blob/master/LICENSE)によって公開されています
- [Xamarin.Forms](https://github.com/xamarin/Xamarin.Forms) : [MIT License](https://github.com/xamarin/Xamarin.Forms/blob/master/LICENSE)によって公開されています。
- [AndroidSupportComponents](https://github.com/xamarin/AndroidSupportComponents) : [MIT License](https://github.com/xamarin/AndroidSupportComponents/blob/master/LICENSE.md)によって公開されています。