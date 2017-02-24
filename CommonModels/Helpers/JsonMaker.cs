using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
