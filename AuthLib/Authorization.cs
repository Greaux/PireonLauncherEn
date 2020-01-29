using AuthLib.JSON;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthLib
{
    public class Authorization
    {
        public static string url = "http://pireon.pro/en";
        public string AuthURL = $"{url}/API/CheckAccount.php?api_key=";
        public const string APIKey = "3ARYGFHYRBTHTYYHGGDG24";
        private readonly string Login;
        private readonly string Password;
        public Authorization(string login, string pass, string url)
        {
            Authorization.url = url;
            Login = login;
            Password = pass;
        }
        public AuthPacket GetPacket()
        {

            JObject auth;
            try
            {
                auth = UJson.GetJSONByURL($"{AuthURL}{APIKey}&user={Login}&password={Password}");
            }
            catch { return new AuthPacket("", "", "error", "", "", "Incorrect login data"); }
            AuthPacket packet;
            if (auth.GetToken("status").ToString() == "error")
                packet = new AuthPacket("", "", "error", "", "", auth.GetToken("error").ToString());
            else if(auth.GetToken("status").ToString() == "success")
                packet = new AuthPacket(auth.GetToken("ip").ToString(), auth.GetToken("port").ToString(), auth.GetToken("status").ToString(), Login, Password);
            else
                packet = new AuthPacket("", "", "", "", "unk");
            return packet;
        }
    }
}
