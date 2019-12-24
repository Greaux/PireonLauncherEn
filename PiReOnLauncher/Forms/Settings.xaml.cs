using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PiReOnLauncher.Code;
using System.Threading;

namespace PiReOnLauncher.Forms
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
                PathTB.Text = dialog.FileName;
        }

        private void Btn2_Click(object sender, RoutedEventArgs e)
        {
            LauncherOptions.GamePath = PathTB.Text;
            PiReOn.INSTANCE.Launcher.LSettings.SaveSettings();
            new Thread(() => PiReOn.INSTANCE.Launcher.LUpdater.InstallCompleted(false)).Start();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PathTB.Text = LauncherOptions.GamePath;
        }
    }
}
