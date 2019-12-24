using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiReOnLauncher.Code
{
    public static class LauncherOptions
    {
        public static string GamePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}/Pireon";
        public static string TempPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/Temp/pTemp_";
    }
}
