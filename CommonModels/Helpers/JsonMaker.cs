using Newtonsoft.Json;

namespace CommonModels.Helpers
{
    public static class JsonMaker
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
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
    }
}
