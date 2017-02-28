using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CommonModels.Helpers
{
    public static class JsonMaker
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj).ToLower();
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
            public string Token { get; set; }
        }
    }
}
