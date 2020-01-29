using AuthLib;
//using NLog;
using PiReOnLauncher.Code;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PiReOnLauncher.Forms
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private Authorization Auth;
        public static string PireonPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Pireon";
        //private readonly Logger Log = LogManager.GetCurrentClassLogger();
        public Login()
        {
            InitializeComponent();
            App.LanguageChanged += LanguageChanged;

            CultureInfo currLang = App.Language;
        }

        private void LanguageChanged(object sender, EventArgs e)
        {

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void LoginAction()
        {
            if (App.Language != App.Languages[0])
            {
                //Сюда вписать свои значения.
                //*EDIT IT*
                _GLOBAL.FTP_LOGIN = "client";
                _GLOBAL.FTP_PASSWORD = "p@ssw0rd131";
                _GLOBAL.GAMEPATH = "PireonRu";
                _GLOBAL.URL_REGION = "http://pireon.pro/ru/";
                _GLOBAL.FTPPIREONURL = "updateru.pireon.pro";
            }
            SwitchAll();
            Checking(true);
            NotifyTB.Text = "";

            new Thread(() =>
            {
                try
                {
                    if (!LUtils.InternetConnection())
                    {
                        ToNotify("Check internet connection!");
                        SwitchAll();
                        Checking(false);
                        return;
                    }
                    if (!LUtils.TestSite(_GLOBAL.URL_REGION))
                    {
                        ToNotify("Site is unavailable, all actual information in social webs");
                        SwitchAll();
                        Checking(false);
                        return;
                    }
                    Auth = new Authorization(this.ReturnIt<string>(new Func<string>(() => LoginField.Text)), this.ReturnIt<string>(new Func<string>(() => PasswordField.Text)), _GLOBAL.URL_REGION);
                    AuthPacket pack = Auth.GetPacket();
                    if (pack.Status == AuthStatus.Success)
                    {
                        SaveLogin();
                        this.InvokeIt(() =>
                        {
                            new PiReOn(pack).Show();
                            Close();
                        });
                    }
                    else
                    {
                        ToNotify(pack.Error);
                        SwitchAll();
                        Checking(false);
                    }
            }
                catch (AggregateException ae)
            {
                ae.Handle(ex =>
                {
                    foreach (var x in ae.InnerExceptions)
                    {
                        Console.WriteLine(x.Message + "\n" + x.ToString());
                    }
                    if (ex is System.Net.Http.HttpRequestException)
                    {
                        ToNotify("Turn off Proxy.");
                        SwitchAll();
                        Checking(false);
                    }
                    return ex is System.Net.Http.HttpRequestException;
                });
            }
            catch { ToNotify("Unknown error"); Checking(false); SwitchAll(); }
        }).Start();
        }
        private void ToNotify(string msg)
        {
            this.InvokeIt(() => { Loading.Visibility = Visibility.Hidden; NotifyTB.Text = msg; });
        }
        private void SaveLogin()
        {
            LUtils.OverrideDirectory(PireonPath);
            string login = LDispatcher.ReturnIt<string>(this, new Func<string>(() => LoginField.Text));
            string password = SProt(LDispatcher.ReturnIt<string>(this, new Func<string>(() => PasswordField.Text)));
            File.WriteAllText($"{PireonPath}/login.dat", $"{login}\n{password}");
        }
        private void LoadLogin()
        {
            if (!Directory.Exists(PireonPath) || !File.Exists($"{PireonPath}/login.dat"))
                return;
            string filetext = File.ReadAllText($"{PireonPath}/login.dat");
            LoginField.Text = filetext.Split('\n')[0];
            PasswordField.Text = SProt(filetext.Split('\n')[1]);
        }
        private string SProt(string pwds)
        {
            string now = "";
            for (int i = 0; i < pwds.Length; i++)
                now += (char)(pwds[i] ^ 16);
            return now;
        }
        private void SwitchAll()
        {
           this.InvokeIt(() => {
               LoginField.IsEnabled = !LoginField.IsEnabled;
               PasswordField.IsEnabled = !PasswordField.IsEnabled;
               ENLOGIN.IsEnabled = !ENLOGIN.IsEnabled;
               RULOGIN.IsEnabled = !RULOGIN.IsEnabled;
           });
        }
        private void Checking(bool turn)
        {
            this.InvokeIt(() => Loading.Visibility = (turn ? Visibility.Visible : Visibility.Hidden));
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadLogin();
        }

        private void SignIn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://pireon.pro/index.php?act=register");
            }
            catch { }
        }

        private void RULOGIN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            App.Language = App.Languages[1];
            LoginAction();
        }

        private void ENLOGIN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            App.Language = App.Languages[0];
            LoginAction();
        }
    }
}
