using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiReOnLauncher.Code
{
    /// <summary>
    /// Базовый класс контроллеров
    /// </summary>
    public abstract class IController
    {
        public Launcher LAUNCHER;
        public IController(Launcher LAUNCHER)
        {
            this.LAUNCHER = LAUNCHER;
        }
    }
}
