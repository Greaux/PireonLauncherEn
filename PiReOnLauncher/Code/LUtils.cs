using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Soap;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PiReOnLauncher.Code
{
    public static class LUtils
    {
        //public static string GetMD5(byte[] array)
        //{
        //    MD5 md5 = new MD5CryptoServiceProvider();
        //    byte[] hashenc = md5.ComputeHash(array);
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var b in hashenc)
        //        sb.Append(b.ToString("x2"));
        //    return sb.ToString();
        //}
        //public static bool EqMD5(byte[] arr1, byte[] arr2)
        //{
        //    return GetMD5(arr1) == GetMD5(arr2);
        //}
        public static void OverrideDirectory(string path)
        {
            if (Directory.Exists(path))
                return;
            Directory.CreateDirectory(path);
        }
        public static void PredictCreateFile(string path, string content = "")
        {
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
                OverrideDirectory(dir);
            File.WriteAllText(path, content);
        }
        public static string SystemDriver()
        {
            string system = Environment.GetFolderPath(Environment.SpecialFolder.System);
            return Path.GetPathRoot(system);
        }
        public static bool InternetConnection()
        {
            try
            {
                Ping p = new Ping();
                PingReply pr = p.Send(@"pireon.pro");
                return pr.Status == IPStatus.Success;
            }
            catch { return false; }
        }
        public static bool TestSite(string url)
        {

            Uri uri = new Uri(url);
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool InternetConnectionToPireon()
        {
            try
            {
                Ping p = new Ping();
                PingReply pr = p.Send(@"pireon.pro");
                return pr.Status == IPStatus.Success;
            }
            catch { return false; }
        }
    }

    public static class Saver
    {
        public static bool Save(Type @class, string filepath)
        {
            try
            {
                FieldInfo[] fields = @class.GetFields(BindingFlags.Static | BindingFlags.Public);
                object[,] a = new object[fields.Length, 2];
                for (int i = 0; i < fields.Length; i++)
                {
                    a[i, 0] = fields[i].Name;
                    a[i, 1] = fields[i].GetValue(null);
                }
                LUtils.PredictCreateFile(filepath, "");
                Stream f = File.Open(filepath, FileMode.Open);
                SoapFormatter soap = new SoapFormatter();
                soap.Serialize(f, a);
                f.Close();
                return true;
            }
            catch { return false; }
        }

        public static bool Load(Type @class, string filepath)
        {
            try
            {
                FieldInfo[] fields = @class.GetFields(BindingFlags.Static | BindingFlags.Public);
                object[,] a;
                Stream f = File.Open(filepath, FileMode.Open);
                SoapFormatter soap = new SoapFormatter();
                a = soap.Deserialize(f) as object[,];
                f.Close();
                //if (a.GetLength(0) != fields.Length) return false;
                for (int i = 0; i < fields.Length; i++)
                    if (fields[i].Name == (a[i, 0] as string) && a[i, 1] != null)
                        fields[i].SetValue(null, a[i, 1]);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
