using System.Text.Json;

namespace WideWorldImporters.Api.IntegrationTests.TestHelpers.Serialization
{
    public static class JsonSerializerHelper
    {
        public static JsonSerializerOptions DefaultSerialisationOptions => new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };

        public static JsonSerializerOptions DefaultDeserialisationOptions => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
