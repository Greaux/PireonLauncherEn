using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthLib.JSON
{
    public static class UJson
    {
        public static JToken GetNamespace(string @namespace, string url)
        {
            return GetJSONByURL(url)[@namespace];
        }

        public static JToken GetNamespace(this JObject obj, object @namespace)
        {
            return obj[@namespace];
        }

        public static JToken GetNamespace(this JArray jArray, object @namespace)
        {
            return jArray[@namespace];
        }

        public static JObject GetJSONByURL(string URL)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", $"Pireon Launcher");
                var content = client.GetStringAsync(URL);
                return JObject.Parse(content.Result);
            }
        }

        public static JArray GetJSONArrayByURL(string URL)
        {
            using (var client = new HttpClient())
            {
                var content = client.GetStringAsync(URL);
                return JArray.Parse(content.Result);
            }

        }

        public static object GetNode(this object obj, object @namespace)
        {
            if (obj.ToString().StartsWith("["))
                return JArray.Parse(obj.ToString())[@namespace];
            else
                return JObject.Parse(obj.ToString())[@namespace];
        }

        public static object GetToken(this JToken token, object ftoken)
        {
            return token[ftoken];
        }

        public static object GetToken(this JObject obj, object token, object @namespace)
        {
            return obj[@namespace][token];
        }
    }
}
