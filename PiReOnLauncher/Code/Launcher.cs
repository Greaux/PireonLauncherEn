using FluentFTP;
//using NLog;
using PiReOnLauncher.Code.FTP;
using PiReOnLauncher.Forms;
using PiReOnLauncher.NewsBlock;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiReOnLauncher.Code
{
    public class Launcher
    {
        //HASH -> текущий HASH лаунчера.
        //public static string Hash { get; set; }
        public LoadingController LController;
        public Updater.Updater LUpdater;
        public FTPController FController;
        public SettingsController LSettings;
        public NewsController LNController;
        public AuthLib.AuthPacket PACKET;
        public Launcher(AuthLib.AuthPacket PACKET)
        {
            this.PACKET = PACKET;
            FController = new FTPController(this, "clienten", "p@ssw0rd131", "updateen.pireon.pro");
            LController = new LoadingController();
            LSettings = new SettingsController(this);
            LUpdater = new Updater.Updater(this);
            LNController = new NewsController();
        }
        public void NotifyWindow(string title, string desc)
        {
            LDispatcher.InvokeIt(() =>
            {
                PopupWindow win = new PopupWindow(PiReOn.INSTANCE, title, desc);
                win.ShowDialog();
            });
        }
        public void UpdateDownloadingStatus(string status)
        {
            LDispatcher.InvokeIt(() =>
            {
                PiReOn.INSTANCE.DStatus.Text = status;
            });
        }
        public void EnableButton(bool en)
        {
            LDispatcher.InvokeIt(() =>
            {
                PiReOn.INSTANCE.DownloadBtn.IsEnabled = en;
            });
        }
        public void EnableButton(bool en, int millis)
        {
            new Thread(() =>
            {
                Thread.Sleep(millis);
                LDispatcher.InvokeIt(() =>
                {
                    PiReOn.INSTANCE.DownloadBtn.IsEnabled = en;
                });
            }).Start();
        }
        /// <summary>
        /// Инициализация лаунчера
        /// </summary>
        public void Init()
        {
            try
            {
                if (!LUtils.InternetConnection())
                {
                    LController.ChangeStatus("Unavailable to connect with Pireon.pro!");
                    return;
                }
                if (!LUtils.TestSite("https://pireon.pro/en/"))
                {
                    LController.ChangeStatus("Oh... Problems with web-site, check socials for info");
                    return;
                }

                LUtils.OverrideDirectory($"{LauncherOptions.TempPath}");
                LUtils.OverrideDirectory(Path.GetDirectoryName(SettingsController.SETTINGSPLACE));

                LController.ChangeStatus("Loading laucher settings");
                LSettings.LoadSettings();

                LController.ChangeStatus("Connecting to Update Service");
                if (!FController.TryConnect())
                {
                    LController.ChangeStatus("Can not connect to Update Service");
                    while (!FController.TryConnect())
                    {
                        LController.ChangeStatus("Trying to connect to Update Service again...");
                        Thread.Sleep(1000);
                    }
                }

                LController.ChangeStatus("Getting information about client");
                LDispatcher.InvokeIt(() => PiReOn.INSTANCE.LVersion.Text = $"Installed version: {(LUpdater.CurrentVersion.ToString() == "0.0.0" ? "?" : LUpdater.CurrentVersion.ToString())}");
                LDispatcher.InvokeIt(() => PiReOn.INSTANCE.AVersion.Text = $"Actual version {LUpdater.ActualVersion}");

                LNController.AddNews();

                GoCheckGameStatus();

                LDispatcher.InvokeIt(() => LController.SwitchLoading());
            }
            catch {}
        }
        private void GoCheckGameStatus()
        {
            Updater.GameStatus status = LUpdater.CheckGame();
            switch (status)
            {
                case Updater.GameStatus.None: //Игра не найдена.
                    break;
                case Updater.GameStatus.Ready://Игра готова к запуску
                    LUpdater.ProgressBarChangeValue(LUpdater.ProgressBarGetMaximum());
                    LUpdater.ButtonSetValue("Play!");
                    break;
                case Updater.GameStatus.UnUpdate: //Игра требует обновления
                    LUpdater.ProgressBarChangeValue(0);
                    LUpdater.ButtonSetValue("Update");
                    break;
            }
        }
    }
}
