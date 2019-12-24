using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PiReOnLauncher.Code
{
    /// <summary>
    /// Контроллер загрузки
    /// </summary>
    public class LoadingController
    {
        public LoadingController()
        {

        }
        /// <summary>
        /// Изменение статуса загрузки лаунчера на пользовательскую
        /// </summary>
        /// <param name="msg"></param>
        public void ChangeStatus(string msg) => LDispatcher.InvokeIt(() => { PiReOn.INSTANCE.LStatus.Text = msg;} );
        public void SwitchLoading()
        {
            LDispatcher.InvokeIt(() =>
            {
                PiReOn.INSTANCE.Loading.Visibility = (PiReOn.INSTANCE.Loading.Visibility == System.Windows.Visibility.Visible ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible);
                PiReOn.INSTANCE.Character.Visibility = (PiReOn.INSTANCE.Character.Visibility == System.Windows.Visibility.Visible ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible);
            });
        }
    }
}
