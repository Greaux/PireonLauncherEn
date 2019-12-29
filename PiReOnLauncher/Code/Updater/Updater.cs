//using NLog;
using PiReOnLauncher.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PiReOnLauncher.Code.Updater
{
    public enum GameStatus
    {
        None,
        UnUpdate,
        Ready
    }
    public class Updater : IController
    {
        private BrushConverter Converter;
        public UpdateStrategy Strategy;
        //private Logger Log = LogManager.GetCurrentClassLogger();
        //public Thread ProgressThread;
        public Updater(Launcher l) : base(l) { Converter = new BrushConverter(); Strategy = new UpdateStrategy(this); }
        /// <summary>
        /// Получение текущей версии клиента
        /// </summary>
        public GameVersion CurrentVersion
        {
            get
            {
                try
                {
                    if (!Directory.Exists(LauncherOptions.GamePath))
                        return new GameVersion(0, 0, 0);
                    return new GameVersion(File.ReadAllText($"{LauncherOptions.GamePath}/system/ClientVersion.ini"));
                }
                catch { return new GameVersion(0, 0, 0); }
            }
        }
        /// <summary>
        /// Запрос на получение актуальной версии клиента
        /// </summary>
        public GameVersion ActualVersion
        {
            get
            {
                return new GameVersion(LAUNCHER.FController.PatchInfo());
            }
        }
        /// <summary>
        /// Основная функция, где происходит проверка клиента игры
        /// </summary>
        public void MainProcess()
        {
            try
            {
                GameStatus stat = CheckGame();
                IUpdate update;
                switch (stat)
                {
                    case GameStatus.None:
                        LUtils.OverrideDirectory(LauncherOptions.GamePath);
                        update = new DownloadClient(this);
                        break;
                    case GameStatus.UnUpdate:
                        update = new UpdateClient(this);
                        break;
                    case GameStatus.Ready:
                        update = new GameStarter(this);
                        //Check hash todo?
                        break;
                    default:
                        throw new Exception("Unknown status.");
                }
                Strategy.SetUpdate(update);
                Strategy.Execute();
            }
            catch(Exception ex) {/* Log.Error(ex);*/ }
        }
        /// <summary>
        /// Срабатывает при установке клиента.
        /// </summary>
        /// <param name="x"></param>
        public void InstallCompleted(bool x = true)
        {
            LAUNCHER.LController.SwitchLoading();
            LAUNCHER.LController.ChangeStatus("Rebuilding launcher...");

            LDispatcher.InvokeIt(() => PiReOn.INSTANCE.LVersion.Text = $"Installed version: {(CurrentVersion.ToString() == "0.0.0" ? "?" : CurrentVersion.ToString())}");
            LDispatcher.InvokeIt(() => PiReOn.INSTANCE.AVersion.Text = $"Actual verison: {ActualVersion.ToString()}");
            GameStatus stat = CheckGame();
            switch (stat)
            {
                case GameStatus.None:
                    ButtonSetValue("Download game");
                    break;
                case GameStatus.Ready:
                    ButtonSetValue("Play!");
                    break;
                case GameStatus.UnUpdate:
                    ButtonSetValue("Update");
                    break;
            }

            Thread.Sleep(500);

            if(x)
                LAUNCHER.EnableButton(true, 2500);

            LAUNCHER.LController.SwitchLoading();
        }
        public GameStatus CheckGame()
        {
            if (!Directory.Exists(LauncherOptions.GamePath) || Directory.GetFiles(LauncherOptions.GamePath, "*", SearchOption.AllDirectories).Length == 0)
                return GameStatus.None;
            else if (!LAUNCHER.LUpdater.IsActual())
                return GameStatus.UnUpdate;
            else
                return GameStatus.Ready;
        }
        public void ButtonSetValue(string msg) => LDispatcher.InvokeIt(() => PiReOn.INSTANCE.DownloadBtn.Content = msg);
        public void ProgressBarChangeValue(double value)
        {
            LDispatcher.InvokeIt(() => PiReOn.INSTANCE.Pbar.Value = value);
        }
        public double ProgressBarGetMaximum() => LDispatcher.ReturnIt<double>(new Func<double>(() => PiReOn.INSTANCE.Pbar.Maximum));
        public void ProgressBarChangeMaximum(int value)
        {
            LDispatcher.InvokeIt(() => PiReOn.INSTANCE.Pbar.Maximum = value);
        }
        public void ProgressBarChangeColor(string color)
        {
            LDispatcher.InvokeIt(() => PiReOn.INSTANCE.Pbar.Foreground = Converter.ConvertFromString(color) as Brush);
        }
        public bool IsActual()
        {
            return CurrentVersion == ActualVersion;
        }
        /// <summary>
        /// Проверяет лаунчер на целостность
        /// </summary>
        public void CheckLauncher()
        {
            byte[] launcher;
            LAUNCHER.FController.FTP.Download(out launcher, "/launcher/PireOnEn.exe");
            if (launcher is null)
                return;
            //todo launcher update
        }
    }
}
