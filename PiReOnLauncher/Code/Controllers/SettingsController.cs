using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiReOnLauncher.Code
{
    /// <summary>
    /// Контроллер отвечающий за настройку лаунчера, клиента
    /// </summary>
    public class SettingsController : IController
    {
        public SettingsController(Launcher L) : base(L) { }
        public static string SETTINGSPLACE = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Pireon/cfg.launch";
        /// <summary>
        /// Загрузка настроек лаунчера
        /// </summary>
        public void LoadSettings() => Saver.Load(typeof(LauncherOptions), SETTINGSPLACE);
        /// <summary>
        /// Сохранение настроек лаунчера
        /// </summary>
        public void SaveSettings() => Saver.Save(typeof(LauncherOptions), SETTINGSPLACE);
    }
}
