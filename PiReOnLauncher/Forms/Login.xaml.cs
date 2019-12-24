using AuthLib;
//using NLog;
using PiReOnLauncher.Code;
using System;
using System.Collections.Generic;
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
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void LogInBtn_Click(object sender, RoutedEventArgs e)
        {
            SwitchAll();
            NotifyTB.Text = "";
            new Thread(() =>
            {
                try
                {
                    if (!LUtils.InternetConnection())
                    {
                        ToNotify("Проверьте подключение к интернету!");
                        return;
                    }
                    if (!LUtils.TestSite("http://pireon.ru"))
                    {
                        ToNotify("Сайт недоступен, вся актуальная информации в группе ВК.");
                        return;
                    }
                    Auth = new Authorization(this.ReturnIt<string>(new Func<string>(() => LoginField.Text)), this.ReturnIt<string>(new Func<string>(() => PasswordField.Text)));
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
                    }
                }
                catch (AggregateException ae) {
                    ae.Handle(ex =>
                    {
                        if (ex is System.Net.Http.HttpRequestException)
                        {
                            ToNotify("Отключите прокси.");
                            SwitchAll();
                        }
                        return ex is System.Net.Http.HttpRequestException;
                    });
                }
                catch {ToNotify("Неизвестная ошибка | (LOG)"); SwitchAll(); }
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
               LogInBtn.IsEnabled = !LogInBtn.IsEnabled;
               Loading.Visibility = (Loading.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible);
           });
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
                System.Diagnostics.Process.Start("http://pireon.ru/index.php?act=register");
            }
            catch { }
        }
    }
}
