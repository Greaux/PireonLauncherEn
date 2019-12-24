//using NLog;
using PiReOnLauncher.Code;
using PiReOnLauncher.Code.Updater;
using PiReOnLauncher.NewsBlock;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PiReOnLauncher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class PiReOn : Window
    {
        public static PiReOn INSTANCE;
        public Launcher Launcher;
        public AuthLib.AuthPacket PACKET;
        //private readonly Logger Log = LogManager.GetCurrentClassLogger();
        public PiReOn(AuthLib.AuthPacket Packet)
        {
            InitializeComponent();
            INSTANCE = this;
            PACKET = Packet;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Launcher = new Launcher(PACKET);
            new Thread(() => Launcher.Init()).Start();
            new Thread(() => LDispatcher.InvokeIt(() => XamlAnimatedGif.AnimationBehavior.SetSourceUri(GifBG, new Uri("pack://application:,,,/PiReOnLauncher;component/Content/bg.gif")))).Start();
        }
        private void VisitSite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start("http://pireon.pro/");
            }
            catch/*(Exception ex)*/ { /*Log.Error(ex);*/ }
        }

        private void VisitVK_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start("https://vk.com/pireon");
            }
            catch/*(Exception ex)*/ { /*Log.Error(ex);*/ }
        }
        private void VisitSite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start("http://pireon.ru/");
            }
            catch/*(Exception ex)*/ { /*Log.Error(ex);*/ }
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Forms.Settings Sett = new Forms.Settings();
            Sett.ShowDialog();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => Launcher.LUpdater.MainProcess()).Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Launcher.LUpdater.ProgressThread.Interrupt();
        }
    }
}
