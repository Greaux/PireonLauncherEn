using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PiReOnLauncher.Code.Updater
{
    /// <summary>
    /// Возможна замена на стандартный 'Version'
    /// </summary>
    /// 
    public class GameVersion
    {
        private Regex VersionRegex = new Regex(@"([0-9]+)\.([0-9]+)\.([0-9]+)");
        public string Version { get; set; }
        public int Major;
        public int Minor;
        public int Build;
        public GameVersion(string version)
        {
            Version = version;
            ParseVersion();
        }
        public GameVersion(int major, int minor, int build)
        {
            Major = major;
            Minor = minor;
            Build = build;
        }
        private void ParseVersion()
        {
            if (string.IsNullOrEmpty(Version))
                return;
            Match m = VersionRegex.Match(Version);
            Major = int.Parse(m.Groups[1].Value);
            Minor = int.Parse(m.Groups[2].Value);
            Build = int.Parse(m.Groups[3].Value);
        }
        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}";
        }
        public static bool operator <(GameVersion v1, GameVersion v2)
        {
            bool flag = false;
            if (v1.Major < v2.Major)
                return true;
            else if (v1.Major > v2.Major)
                return false;
            else if (v1.Major == v2.Major)
                flag = false;
            if (v1.Minor < v2.Minor)
                return true;
            else if (v1.Minor > v2.Minor)
                return false;
            else if (v1.Minor == v2.Minor)
                flag = false;
            if (v1.Build < v2.Build)
                flag = true;
            else if (v1.Build > v2.Build)
                flag = false;
            else if (v1.Build == v2.Build)
                flag = false;
            return flag;
        }

        public static bool operator ==(GameVersion v1, GameVersion v2)
        {
            return v1.ToString() == v2.ToString();
        }
        public static bool operator !=(GameVersion v1, GameVersion v2)
        {
            return v1.ToString() != v2.ToString();
        }
        public static bool operator >(GameVersion v1, GameVersion v2)
        {
            bool flag = false;
            if (v1.Major > v2.Major)
                return true;
            else if (v1.Major < v2.Major)
                return false;
            else if (v1.Major == v2.Major)
                flag = false;
            if (v1.Minor > v2.Minor)
                return true;
            else if (v1.Minor < v2.Minor)
                return false;
            else if (v1.Minor == v2.Minor)
                flag = false;
            if (v1.Build > v2.Build)
                flag = true;
            else if (v1.Build < v2.Build)
                flag = false;
            else if (v1.Build == v2.Build)
                flag = false;
            return flag;
        }
    }
}
