using AuthLib.JSON;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthLib
{
    public class Authorization
    {
        public const string AuthURL = "https://pireon.ru/API/CheckAccount.php?api_key=";
        public const string APIKey = "3ARYGFHYRBTHTYYHGGDG24";
        private readonly string Login;
        private readonly string Password;
        public Authorization(string login, string pass)
        {
            Login = login;
            Password = pass;
        }
        public AuthPacket GetPacket()
        {
            JObject auth = UJson.GetJSONByURL($"{AuthURL}{APIKey}&user={Login}&password={Password}");
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
