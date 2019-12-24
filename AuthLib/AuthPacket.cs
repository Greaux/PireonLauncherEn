using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthLib
{
    public enum AuthStatus
    {
        Success,
        Error,
        Unknown
    }
    public class AuthPacket
    {
        public AuthPacket(string IP, string PORT, string status, string login, string password, string error = "")
        {
            this.IP = IP;
            this.PORT = PORT;
            PASSWORD = password;
            LOGIN = login;
            Error = error;
            Status = Parse(status);
        }
        public string IP { get; set; }
        public string PORT { get; set; }
        public string LOGIN { get; set; }
        public string PASSWORD { get; set; }
        public AuthStatus Status { get; set; }
        public string Error { get; set; }
        private AuthStatus Parse(string status)
        {
            switch (status)
            {
                case "success":
                    return AuthStatus.Success;
                case "error":
                    return AuthStatus.Error;
                default:
                    return AuthStatus.Unknown;
            }
        }
    }
}
