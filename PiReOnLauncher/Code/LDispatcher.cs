using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PiReOnLauncher.Code
{
    public static class LDispatcher
    {
        public static void InvokeIt(Action @Action, DispatcherPriority priority = DispatcherPriority.Background) => PiReOn.INSTANCE.Dispatcher.BeginInvoke(priority, @Action);
        public static T ReturnIt<T>(Func<T> func, DispatcherPriority priority = DispatcherPriority.Background) => (T)PiReOn.INSTANCE.Dispatcher.Invoke(priority, func);
        public static void InvokeIt(this Window win, Action @Action, DispatcherPriority priority = DispatcherPriority.Background) => win.Dispatcher.BeginInvoke(priority, @Action);
        public static T ReturnIt<T>(this Window win, Func<T> func, DispatcherPriority priority = DispatcherPriority.Background) => (T)win.Dispatcher.Invoke(priority, func);
    }
}
