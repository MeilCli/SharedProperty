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

値を設定するには`SetProperty`メソッドまたは`SetPropertyAsync`メソッドを利用します。
```csharp
sharedDictionary.SetProperty("text", "sssss");
sharedDictionary.SetProperty("number", 1234);
sharedDictionary.SetProperty("data", new Data());
sharedDictionary.SetProperty("list", new List<int> { 1, 2, 3, 4 });
```
コレクションやユーザー定義型なども利用できますが、各Serializerが利用しているJsonライブラリーによって制限を受けます。

値を取得するには`GetPropety`, `GetPropertyAsync`, `TryGetProperty`, `TryGetPropertyAsync`メソッドを利用します。
```csharp
WriteLine(sharedDictionary.GetProperty<string>("text"));
WriteLine(sharedDictionary.GetProperty<int>("number"));
WriteLine(sharedDictionary.GetProperty<Data>("data"));
WriteLine(sharedDictionary.GetProperty<List<int>>("list").Count);
```
現在のところ型パラメーターは暗黙的な型変換に対応していません。そのため正確な数値型などを指定する櫃夜があります。

### Serializer
#### Utf8Json
Utf8Jsonは一般的な環境ではトップレベルに高速です。

基本的には`Utf8JsonSerializer.Default`を指定すればいいですが、`JsonFormatterResolver`を変更したい場合は`Utf8JsonFormatterResolver`のコンストラクターに渡すように変更してください。  
ユーザー定義型のSerialize方法などは[Utf8Jsonの説明](https://github.com/neuecc/Utf8Json/blob/master/README.md)を参考にしてください。

#### SpanJson
.NET Core 2.1においてはSpanJsonのほうがUtf8Jsonより高速のようです。そのためSharedPropertyはSpanJsonもサポートしています。  
SpanJsonではUTF-16にも対応していますが速度が遅くなるため、UTF-8のみのサポートとしています。

基本的には`SpanJsonSerializer.Default`を指定すればいいですが、`JSonFormatterResolver`を変更したい場合は`SpanJsonFormatterResolver<>`の型パラメーターで指定してください。  
ユーザー定義型のSerizalize方法などは[SpanJsonの説明](https://github.com/Tornhoof/SpanJson/blob/master/README.md)を参考にしてください

### Storage
標準では`FileStorage`と`IsolatedFileStorage`を用意しています。  
アプリケーションデータ固有のデータは`IsolatedFileStorage`の利用を勧めますが、ファイルパスで保存場所を指定したい場合は`FileStorage`を利用してください。

### Converter
難読化や暗号化のために標準ではいくつかのConverterを用意しています。

難読化のためなら`SimpleConverter.Default`, `AesCryptoConverter.Default`, `RijndaelCryptoConverter.Default`が使えます。
`SimpleConverter.Default`は単純に文字コードを+1しているだけなので非常に高速です。ちょっとリッチな難読化をする場合は`AesCryptoConverter.Default`または`RijndaelCryptoConverter.Default`を利用してください。

暗号化には`AesCryptoConverter`または`RijndaelCryptoConverter`が使用できます。  
強度なセキュリティを保つには開発者は必ずアプリケーション固有の暗号鍵を使用する必要があります。

### スレッドセーフ
現在の実装ではasyncメソッドはスレッドセーフな実装です。
しかし同期メソッドはスレッドセーフではない実装なので、混在する使用は気を付ける必要があります。

### 注意
現在の実装では`LoadFromStorageAsync`に失敗したらデータの復元ができません。また、SharedPropertyはデータのマイグレーションに対応していません。  
そのため、ユーザー定義型を保存する際は注意する必要があります。

トークンやIdなどの情報はなるべく基本データ型で保存し、ユーザー定義型は別ファイルを設定したSharedPropertyに保存するのがより安全です。

## ライセンス
このライブラリーは[MIT License](LICENSE.txt)によって公開されています。

また、パッケージによって使用するライブラリーが異なります。

### Serializer.Utf8Json
- [Utf8Json](https://github.com/neuecc/Utf8Json)
  - [MIT License](https://github.com/neuecc/Utf8Json/blob/master/LICENSE)によって公開されています

### Serializer.SpanJson
- [SpanJson](https://github.com/Tornhoof/SpanJson)
  - [MIT License](https://github.com/Tornhoof/SpanJson/blob/master/LICENSE)によって公開されています