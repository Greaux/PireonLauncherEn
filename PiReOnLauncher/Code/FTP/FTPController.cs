using FluentFTP;
//using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiReOnLauncher.Code.FTP
{
    /// <summary>
    /// Контроллер отвечающий за FTP подключение к серверу
    /// </summary>
    public class FTPController : IController
    {
        public FtpClient FTP;
        public readonly string Login;
        public readonly string Password;
        public readonly string URL;
        //private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public FTPController(Launcher l, string usr, string pass, string url) : base(l)
        {
            Login = usr;
            Password = pass;
            URL = url;
            FTP = new FtpClient(URL, 21, new System.Net.NetworkCredential(Login, Password));
            //FTP.OnLogEvent += new Action<FtpTraceLevel, string>((t, x) => Log.Debug(x));
        }
        /// <summary>
        /// Подключение к FTP-серверу
        /// </summary>
        /// <returns></returns>
        public bool TryConnect()
        {
            X509Certificate2 cert_grt = new X509Certificate2();
            FTP.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls | SslProtocols.Tls11;
            FTP.ReadTimeout = 20000;
            FTP.EncryptionMode = FtpEncryptionMode.Explicit;
            FTP.ClientCertificates.Add(cert_grt);
            FTP.ValidateCertificate += new FtpSslValidation(OnValidateCertificate);
            FTP.Connect();
            return FTP.IsConnected;
        }
        /// <summary>
        /// Возвращает путь к файлам в серверной директории 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<string> DownloadListItems(string path)
        {
            using (var conn = new FtpClient(URL, 21, new System.Net.NetworkCredential(Login, Password)))
            {
                foreach (FtpListItem item in conn.GetListing(path))
                    yield return item.FullName;
            }
        }
        /// <summary>
        /// Возвращает путь к файлам в серверной директории
        /// </summary>
        /// <param name="path">FTP-директория</param>
        /// <param name="type">Тип файла</param>
        /// <param name="endswith">(.zip, .rar, .exe, dll, etc.)</param>
        /// <returns></returns>
        public IEnumerable<string> DownloadListItems(string path, FtpFileSystemObjectType type,  string endswith)
        {
            using (var conn = new FtpClient(URL, 21, new System.Net.NetworkCredential(Login, Password)))
            {
                foreach (FtpListItem item in conn.GetListing(path))
                    if (item.Type == type && item.Name.EndsWith(endswith)) {
                        yield return item.FullName;
                    }
            }
        }
        private void OnValidateCertificate(FtpClient control, FtpSslValidationEventArgs e)
        {
            e.Accept = true;
        }

        /// <summary>
        /// Скачивание с FTP-сервера файла с актуальной версией клиента
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string PatchInfo(string url = "/patch/patch.info")
        {
            string patchinfo = "";
            try
            {
                using (var client = new FtpClient(URL, 21, new System.Net.NetworkCredential(Login, Password))) {
                    using (var remoteFileStream = client.OpenRead(url, FtpDataType.ASCII, false))
                    {
                        byte[] buffer = new byte[8 * 1024];
                        int len;
                        while ((len = remoteFileStream.Read(buffer, 0, buffer.Length)) > 0)
                            patchinfo += Encoding.Default.GetString(buffer, 0, len);
                        remoteFileStream.Close();
                        remoteFileStream.Dispose();
                    }
                    client.Dispose();
                }
            }
            catch /*(Exception ex)*/ { /*Log.Error(ex);*/ }
            return patchinfo;
        }
    }
}
