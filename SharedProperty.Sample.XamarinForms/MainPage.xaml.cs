using SharedProperty.NETStandard;
using SharedProperty.NETStandard.Converters;
using SharedProperty.NETStandard.Storage;
using SharedProperty.Serializer.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SharedProperty.Sample.XamarinForms
{
    public partial class MainPage : ContentPage
    {
        private const string welcomeKey = "welcome_key";
        public MainPage()
        {
            InitializeComponent();
            showCount();
        }

        private async void showCount()
        {
            // if you generate code for Xamarin.iOS, you use under.
            /*Utf8Json.Resolvers.CompositeResolver.Register(
                // code generating resolver
                Utf8Json.Resolvers.GeneratedResolver.Instance,
                Utf8Json.Resolvers.BuiltinResolver.Instance,
                Utf8Json.Resolvers.AttributeFormatterResolver.Instance,
                Utf8Json.Resolvers.DynamicGenericResolver.Instance,
                Utf8Json.Resolvers.EnumResolver.Default
            );*/
            // var formatterResolver = new Utf8JsonFormatterResolver(Utf8Json.Resolvers.CompositeResolver.Instance);

            var formatterResolver = new Utf8JsonFormatterResolver(AotStandardResolver.Default);
            var sharedDictionary = new SharedDictionary(
                new Utf8JsonSerializer(formatterResolver),
                IsolatedFileStorage.Default,
                AesCryptoConverter.Default
            );
            await sharedDictionary.LoadFromStorageAsync();

            if (sharedDictionary.TryGetProperty(welcomeKey, out int count))
            {
                WelcomeCounter.Text = $"Welcome Count: {count}";
            }
            else
            {
                WelcomeCounter.Text = $"First Welcome!";
            }

            count++;

            sharedDictionary.SetProperty(welcomeKey, count);

            await sharedDictionary.SaveToStorageAsync();
        }
    }
}
