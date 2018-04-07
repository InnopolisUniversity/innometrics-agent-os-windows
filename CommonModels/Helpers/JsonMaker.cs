using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CommonModels.Helpers
{
    public static class JsonMaker
    {
        public static string Serialize(object obj)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new LowercaseContractResolver();
            return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
        }
        
        public static string DeserializeToken(string tokenJson)
        {
            TokenDeserializationHelpClass t = JsonConvert.DeserializeObject<TokenDeserializationHelpClass>(tokenJson);
            return t.Token;
        }


        // ----------------------------------------
        // Helper classes
        private class TokenDeserializationHelpClass
        {
            public TokenDeserializationHelpClass(string token)
            {
                Token = token;
            }

            public string Token { get; }
        }

        private class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }
    }
}
