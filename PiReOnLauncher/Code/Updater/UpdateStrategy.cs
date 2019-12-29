using FluentFTP;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace PiReOnLauncher.Code.Updater
{
    //Паттерн 'Strategy'
    public class UpdateStrategy
    {
        private IUpdate update;
        private readonly Updater upd;
        public UpdateStrategy(Updater upd)
        {
            this.upd = upd;
        }
        public UpdateStrategy(Updater upd, IUpdate strategy)
        {
            this.upd = upd;
            update = strategy;
        }
        public void SetUpdate(IUpdate update) => this.update = update;
        public void Execute() {
            if (!LUtils.InternetConnectionToPireon())
            {
                upd.LAUNCHER.NotifyWindow("Error!", "No connection with Pireon.pro!");
                upd.LAUNCHER.EnableButton(true);
                return;
            }
            LUtils.OverrideDirectory($"{LauncherOptions.TempPath}");
            LUtils.OverrideDirectory($"{LauncherOptions.TempPath}/patches");
            upd.LAUNCHER.EnableButton(false);
            new Thread(() => update.Process()) { IsBackground = true }.Start();
        }
    }
    public abstract class IUpdate
    {
        public static Updater UPDATER;
        public IUpdate(Updater upd) { UPDATER = upd;}
        public abstract void Process();
    }
    public class DownloadClient : IUpdate
    {
        //private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public DownloadClient(Updater upd) : base(upd){}
        Action<FtpProgress> progress = new Action<FtpProgress>(x =>
        {
            UPDATER.LAUNCHER.UpdateDownloadingStatus($"Downloading client... ({x.TransferSpeedToString()})");
            UPDATER.ProgressBarChangeValue(x.Progress);
        });
        public override void Process() 
        {
            try
            {
                long ClientSize = UPDATER.LAUNCHER.FController.FTP.GetFileSize("/client.zip");
                if (!HasSpace(ClientSize))
                {
                    UPDATER.LAUNCHER.NotifyWindow("Error", $"Not enough space on drive '{LUtils.SystemDriver()}'");
                    return;
                }
                UPDATER.LAUNCHER.FController.FTP.DownloadFile($"{LauncherOptions.TempPath}/client.zip", "/client.zip", FtpLocalExists.Append, FtpVerify.Retry, progress);
                ExtractClient();
            }
            catch { UPDATER.LAUNCHER.EnableButton(true); }
        }
        private static void ExtractClient()
        {
            try
            {
                //Log.Debug("Beginning extracting");
                UPDATER.ProgressBarChangeValue(0);
                UPDATER.ProgressBarChangeColor("#E16A00");
                using (ZipFile file = ZipFile.Read($"{LauncherOptions.TempPath}/client.zip"))
                {
                    UPDATER.ProgressBarChangeMaximum(file.Count);
                    file.ExtractProgress += new EventHandler<ExtractProgressEventArgs>(ClientExtractProgress);
                    file.ExtractAll(LauncherOptions.GamePath);
                }
            }
            catch { UPDATER.LAUNCHER.EnableButton(true); }
        }
        private static void ClientExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            try
            {
                if (e.CurrentEntry == null || e.CurrentEntry.IsDirectory || e.EntriesExtracted == 0)
                    return;
                UPDATER.LAUNCHER.UpdateDownloadingStatus($"Unpacking game files {e.EntriesExtracted}/{e.EntriesTotal}");
                UPDATER.ProgressBarChangeValue(e.EntriesExtracted);
                if (e.EntriesExtracted == e.EntriesTotal)
                {
                    UPDATER.LAUNCHER.UpdateDownloadingStatus($"Unpacking filished!");
                    UPDATER.LAUNCHER.LUpdater.InstallCompleted();
                }
            }
            catch { UPDATER.LAUNCHER.EnableButton(true); }
        }
        private bool HasSpace(long size)
        {
            DriveInfo SystemDriver = DriveInfo.GetDrives().Where(disk => disk.Name == Path.GetPathRoot(LauncherOptions.GamePath)).FirstOrDefault();
            return SystemDriver.AvailableFreeSpace > size;
        }
    }
    public class UpdateClient : IUpdate
    {
        //private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        Action<FtpProgress> progress = new Action<FtpProgress>(x =>
        {
            UPDATER.LAUNCHER.UpdateDownloadingStatus($"Downloading updates... ({x.TransferSpeedToString()})");
            UPDATER.ProgressBarChangeValue(x.Progress);
        });
        public UpdateClient(Updater upd) : base(upd) { }
        public override void Process() 
        {
            try
            {
                //Log.Debug("_update client process_");
                List<string> KarchPaths = UPDATER.LAUNCHER.FController.DownloadListItems("/patch/", FtpFileSystemObjectType.File, ".zip").ToList();
                foreach (var file in KarchPaths)
                {
                    GameVersion FileVersion = new GameVersion(Path.GetFileName(file));
                    if (FileVersion < UPDATER.CurrentVersion || FileVersion == UPDATER.CurrentVersion)
                        continue;
                    UPDATER.LAUNCHER.FController.FTP.DownloadFile($"{LauncherOptions.TempPath}/patches/{Path.GetFileName(file)}", file, FtpLocalExists.Overwrite, FtpVerify.Delete, progress);
                }
                new Thread(() => ExtractPatches()).Start();
            }
            catch { UPDATER.LAUNCHER.EnableButton(true); }
        }
        private static void ExtractPatches()
        {
            try
            {
                UPDATER.ProgressBarChangeColor("#E16A00");
                foreach (var patch in Directory.GetFiles($"{LauncherOptions.TempPath}/patches", "*.zip"))
                {
                    UPDATER.ProgressBarChangeValue(0);
                    using (ZipFile file = ZipFile.Read(patch))
                    {
                        UPDATER.LAUNCHER.UpdateDownloadingStatus($"Installing updates{Path.GetFileName(patch)}");
                        UPDATER.ProgressBarChangeMaximum(file.Count);
                        file.ExtractProgress += new EventHandler<ExtractProgressEventArgs>(ClientExtractProgress);
                        file.ExtractAll(LauncherOptions.GamePath, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
                UPDATER.LAUNCHER.UpdateDownloadingStatus($"Installing done!");
                UPDATER.LAUNCHER.LUpdater.InstallCompleted();
            }
            catch { UPDATER.LAUNCHER.EnableButton(true); }
        }
        private static void ClientExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            try
            {
                if (e.CurrentEntry == null || e.CurrentEntry.IsDirectory || e.EntriesExtracted == 0)
                    return;
                UPDATER.LAUNCHER.UpdateDownloadingStatus($"Unpacking game files {e.EntriesExtracted}/{e.EntriesTotal}");
                UPDATER.ProgressBarChangeValue(e.EntriesExtracted);
            }
            catch { UPDATER.LAUNCHER.EnableButton(true); }
        }
    }
    public class GameStarter : IUpdate
    {
        public GameStarter(Updater upd) : base(upd) { }
        public override void Process()
        {
            StartGame();
        }
        private void StartGame()
        {
            //startgame {UPDATER.LAUNCHER.PACKET.IP} {UPDATER.LAUNCHER.PACKET.PORT} {UPDATER.LAUNCHER.PACKET.LOGIN} {UPDATER.LAUNCHER.PACKET.PASSWORD}
            string ip = UPDATER.LAUNCHER.PACKET.IP;
            // string port = UPDATER.LAUNCHER.PACKET.PORT;
            // string login = UPDATER.LAUNCHER.PACKET.LOGIN;
            // string password = UPDATER.LAUNCHER.PACKET.PASSWORD;
            System.Diagnostics.Process proc = new Process();
            proc.StartInfo.FileName = $"{LauncherOptions.GamePath}/system/Game.exe";
            // proc.StartInfo.Arguments = $"startgame {ip} {port} {login} {password}"; old one
            proc.StartInfo.Arguments = $"startgame ip:{ip}";
            proc.StartInfo.WorkingDirectory = $"{LauncherOptions.GamePath}";
            proc.Start();
            proc.WaitForExit();
            UPDATER.LAUNCHER.EnableButton(true);
        }
    }
}
