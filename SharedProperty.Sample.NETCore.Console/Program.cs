using SharedProperty.NETStandard;
using SharedProperty.NETStandard.Storage;
using SharedProperty.Serializer.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static System.Console;

namespace SharedProperty.Sample.NETCore.Console
{
    public class Data
    {
        [DataMember(Name = "name")]
        public string Name { get; set; } = "name";

        [DataMember(Name = "id")]
        public int Id { get; set; } = 992;
    }

    class Program
    {
        public static async Task Main(string[] args)
        {
            var sharedDictionary = new SharedDictionary(Utf8JsonSerializer.Default, FileStorage.Default, null);
            sharedDictionary.SetProperty("text", "sssss");
            sharedDictionary.SetProperty("number", 1234);
            sharedDictionary.SetProperty("data", new Data());
            sharedDictionary.SetProperty("list", new List<int> { 1, 2, 3, 4 });
            await sharedDictionary.SaveToStorageAsync();

            sharedDictionary = new SharedDictionary(Utf8JsonSerializer.Default, FileStorage.Default, null);
            await sharedDictionary.LoadFromStorageAsync();

            WriteLine(sharedDictionary.GetProperty<string>("text"));
            WriteLine(sharedDictionary.GetProperty<int>("number"));
            WriteLine(sharedDictionary.GetProperty<Data>("data"));
            WriteLine(sharedDictionary.GetProperty<List<int>>("list").Count);
        }
    }
}
